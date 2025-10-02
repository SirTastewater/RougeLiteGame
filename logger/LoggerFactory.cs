using System;
using System.Collections.Generic;
using Godot;

namespace RougeLiteGame.logger;

/// <summary>
/// Factory class for creating and getting logger instances.
/// </summary>
public class LoggerFactory
{
    private static readonly ConsoleLogger Logger = new(typeof(LoggerFactory));
    
    private static readonly Dictionary<Type, ILogger> Loggers = new();

    /// <summary>
    /// Retrieves an instance of <see cref="ILogger"/> for the specified type.
    /// </summary>
    /// <param name="type">
    /// The type for which the logger is to be retrieved. This is used to associate
    /// log messages with the appropriate context.
    /// </param>
    /// <returns>
    /// An implementation of <see cref="ILogger"/> associated with the specified type.
    /// </returns>
    public static ILogger GetLogger(Type type)
    {
        Logger.Log("Getting logger for type {}.", type);
        if (Loggers.TryGetValue(type, out ILogger value)) return value;
        
        value = new ConsoleLogger(type);
        Loggers.Add(type, value);
        return value;
    }
}