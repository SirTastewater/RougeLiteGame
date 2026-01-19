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
    private static readonly BasicLogger Logger = new(typeof(LoggerFactory), ConsoleLogWriter);
    private static readonly Dictionary<Type, ILogger> Loggers = new();
    private static readonly ISet<IAsyncLogger> AsyncLoggers = new HashSet<IAsyncLogger>();

    public static readonly AsyncWorker AsyncWorker = new(TimeSpan.FromMilliseconds(250));

    /// <summary>
    ///     Retrieves an instance of <see cref="ILogger" /> for the specified type.
    /// </summary>
    /// <param name="type">
    ///     The type for which the logger is to be retrieved. This is used to associate
    ///     log messages with the appropriate context.
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

        value = async ? new BasicAsyncLogger(type, ConsoleLogWriter) : new BasicLogger(type, ConsoleLogWriter);
        Loggers.Add(type, value);

        if (value is IAsyncLogger asyncLogger)
        {
            AsyncLoggers.Add(asyncLogger);
        }
        
        return value;
    }
    
    public static void GlobalFlush()
    {
        foreach (ILogger loggersValue in Loggers.Values) loggersValue.Flush();
    }
    
    public static void GlobalAsyncFlush()
    {
        foreach (IAsyncLogger loggersValue in AsyncLoggers) loggersValue.Flush();
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
}