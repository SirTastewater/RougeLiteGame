using System;
using System.Collections.Generic;
using Godot;

namespace RougeLiteGame.logger;

public class LoggerFactory
{
    private static readonly Dictionary<Type, ILogger> Loggers = new();

    public static ILogger GetLogger(Type type)
    {
        Loggers[type] ??= new ConsoleLogger(type);
        return Loggers[type];
    }
}