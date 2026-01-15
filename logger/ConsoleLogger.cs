using System;
using System.Text;
using Godot;

namespace RougeLiteGame.logger;

public class ConsoleLogger(Type type) : BasicLogger(type)
{
    private readonly StringBuilder _stringBuilder = new(1024);

    public override void Flush()
    {
        Output(Drain());
    }

    private void Output(LogEntry[] logEntries)
    {
        if(logEntries == null || logEntries.Length == 0) return;
        
        _stringBuilder.Clear();

        for (int i = 0; i < logEntries.Length; i++)
        {
            logEntries[i].Interpolate();
            logEntries[i].Render();
            
            string color = ILogger.LogLevelToColor(logEntries[i].Level);

            _stringBuilder.Append("[color=");
            _stringBuilder.Append(color);
            _stringBuilder.Append(']');
            _stringBuilder.Append(logEntries[i].Message);

            if (logEntries[i].Throwable == null)
            {
                if (i != logEntries.Length - 1) { _stringBuilder.AppendLine(); }
                continue;
            }

            _stringBuilder.AppendLine();

            string traceColor = ILogger.LogLevelToColor(LogLevel.Trace);
            _stringBuilder.Append("[color=");
            _stringBuilder.Append(traceColor);
            _stringBuilder.Append(']');
            _stringBuilder.Append(logEntries[i].Throwable);
        }
        
        GD.PrintRich(_stringBuilder.ToString());
    }
}