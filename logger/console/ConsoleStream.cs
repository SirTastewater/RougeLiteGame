
using System.Text;
using Godot;

namespace RougeLiteGame.logger.console;

public static class ConsoleStream
{
    private static readonly StringBuilder StringBuilder = new(1024);
    
    public static void Output(LogEntry[] message)
    {
        StringBuilder.Clear();
        
        foreach (LogEntry command in message)
        {
            command.Interpolate();
            command.Render();
                    
            string color = ILogger.LogLevelToColor(command.Level);

            StringBuilder.Append("[color=");
            StringBuilder.Append(color);
            StringBuilder.Append(']');
            StringBuilder.AppendLine(command.Message);

            if (command.Throwable == null) continue;

            string traceColor = ILogger.LogLevelToColor(LogLevel.Trace);
            StringBuilder.Append("[color=");
            StringBuilder.Append(traceColor);
            StringBuilder.Append(']');
            StringBuilder.AppendLine(command.Throwable);
        }
        
        GD.PrintRich(StringBuilder.ToString());
    }
}