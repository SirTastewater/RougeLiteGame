using System;
using Godot;

namespace RougeLiteGame.logger;

public class ConsoleLogger(Type type) : BasicLogger(type)
{
    private readonly Type _type = type;

    protected override void Out(LogLevel level, string message, Exception throwable = null)
    {
        string outMessage = $"{DateTime.Now} {level.ToString().ToUpper()} [{_type}] {message}";
        GD.PrintRich($"[color={LogLevelToColor(level)}]{outMessage}");
        
        if (throwable != null)
        {
            GD.PrintRich($"[color={LogLevelToColor(LogLevel.Trace)}]{throwable.StackTrace}");
        }
    }

    private static string LogLevelToColor(LogLevel level)
    {
        // used LTRO-1 Palette
        // https://lospec.com/palette-list/ltro-1
        
        return level switch
        {
            LogLevel.Trace    => "#37313b", // dark-grey 
            LogLevel.Debug    => "#4159cb", // blue
            LogLevel.Fine     => "#59a7af", // cyan
            LogLevel.Info     => "#eae1f0", // light gray
            LogLevel.Success  => "#8d902e", // green
            LogLevel.Warn     => "#fdbb27", // yellow
            LogLevel.Error    => "#7e7185", // lighter gray
            LogLevel.Critical => "#89423f", // matt red
            LogLevel.Fatal    => "#f63f4c", // burning eyes red
            _ => "#FFFFFF"
        };
    }
}