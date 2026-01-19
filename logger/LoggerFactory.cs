using System;
using System.Collections.Generic;
using RougeLiteGame.logger.async;

namespace RougeLiteGame.logger;

/// <summary>
///     Factory class for creating and getting logger instances.
/// </summary>
public class LoggerFactory
{
    private static readonly ConsoleLogWriter ConsoleLogWriter = new();
    private static readonly BasicLogRenderer BasicLogRenderer = new();
    
    private static readonly BasicLogger Logger = new(typeof(LoggerFactory), BasicLogRenderer, ConsoleLogWriter);
    private static readonly Dictionary<Type, ILogger> Loggers = new();
    private static readonly HashSet<IAsyncLogger> AsyncLoggers = [];

    public static readonly AsyncWorker AsyncWorker = new(TimeSpan.FromMilliseconds(250));

    /// <summary>
    ///     Retrieves an instance of <see cref="ILogger" /> for the specified type.
    /// </summary>
    /// <param name="type">
    ///     The type for which the logger is to be retrieved. This is used to associate
    ///     log messages with the appropriate context.
    /// </param>
    /// <param name="async">
    ///     Whether the Logger should be flushed automatically by the asynchronous worker 
    /// </param>
    /// <returns>
    ///     An implementation of <see cref="ILogger" /> associated with the specified type.
    /// </returns>
    public static ILogger GetLogger(Type type, bool async = true)
    {
        Logger.Trace("Getting logger for type {}.", type.Name);
        if (Loggers.TryGetValue(type, out ILogger value))
        {
            return value;
        }

        value = async ? new BasicAsyncLogger(type, BasicLogRenderer, ConsoleLogWriter) : new BasicLogger(type, BasicLogRenderer, ConsoleLogWriter);
        Loggers.Add(type, value);

        if (value is IAsyncLogger asyncLogger)
        {
            AsyncLoggers.Add(asyncLogger);
        }
        
        return value;
    }

    /// <summary>
    ///     Retrieves an instance of <see cref="ILogger" /> for the specified generic type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type for which the logger is to be retrieved. This is often used to associate
    ///     log messages with the context of the specified type.
    /// </typeparam>
    /// <returns>
    ///     An implementation of <see cref="ILogger" /> associated with the specified type <typeparamref name="T" />.
    /// </returns>
    public static ILogger GetLogger<T>(bool async = true)
    {
        return GetLogger(typeof(T), async);
    }

    /// <summary>
    /// Flushes all registered synchronous loggers.
    /// </summary>
    /// <remarks>
    /// This method iterates over all loggers that were previously registered via
    /// <c>GetLogger</c> and invokes <see cref="ILogger.Flush"/> on each one.
    /// <para>
    /// The flush operation is performed synchronously and blocks the calling thread
    /// until all loggers have completed their flush.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Any exception thrown by an individual logger's <c>Flush</c> implementation
    /// will propagate to the caller.
    /// </exception>
    public static void GlobalFlush()
    {
        foreach (ILogger loggersValue in Loggers.Values) loggersValue.Flush();
    }

    /// <summary>
    /// Flushes all registered asynchronous loggers using an asynchronous worker.
    /// </summary>
    /// <remarks>
    /// This method iterates over all loggers that were previously registered via
    /// <c>GetLogger</c> and invokes <see cref="IAsyncLogger.Flush"/> on each one.
    /// <para>
    /// While the loggers themselves may not be inherently asynchronous, their
    /// <c>Flush</c> calls are executed by an asynchronous worker to avoid blocking
    /// the main thread. It should only be called asynchronous.
    /// </para>
    /// <para>
    /// This method does not guarantee that all flush operations have completed
    /// when it returns.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Any exception thrown by an individual logger's <c>Flush</c> implementation
    /// may be surfaced by the asynchronous worker.
    /// </exception>
    public static void GlobalAsyncFlush()
    {
        foreach (IAsyncLogger loggersValue in AsyncLoggers) loggersValue.Flush();
    }
}