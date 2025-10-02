using System;
using System.Diagnostics;
using Godot;

namespace RougeLiteGame.logger;

public class ConsoleLogger(Type type) : BasicLogger(type)
{
    private readonly Type _type = type;

    protected override void Out(LogLevel level, string message, Exception throwable = null)
    {
        string outMessage = $"${DateTime.Now} ${level.ToString().ToUpper()} [${_type}] {message})";
        GD.PrintRich(outMessage);
        
        if(throwable != null) Debug.Print(throwable.StackTrace);
    }
}