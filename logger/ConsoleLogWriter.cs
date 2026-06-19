using System.Text;
using Godot;

namespace RougeLiteGame.logger;

public class ConsoleLogWriter : ILogWriter
{
    private readonly StringBuilder _stringBuilder = new(1024);

    public void Write(LogEntry[] buffered)
    {
        if(buffered == null || buffered.Length == 0) return;
        
        _stringBuilder.Clear();

        for (int i = 0; i < buffered.Length; i++)
        {
            buffered[i].Render();
            
            string color = ILogger.LogLevelToColor(buffered[i].Level);

            _stringBuilder.Append("[color=");
            _stringBuilder.Append(color);
            _stringBuilder.Append(']');
            _stringBuilder.Append(buffered[i].Message);

            
            if (buffered[i].Throwable == null)
            {
                if (i != buffered.Length - 1) { _stringBuilder.Append('\n'); }
                continue;
            }

            _stringBuilder.Append('\n');

            string traceColor = ILogger.LogLevelToColor(LogLevel.Trace);
            _stringBuilder.Append("[color=");
            _stringBuilder.Append(traceColor);
            _stringBuilder.Append(']');
            _stringBuilder.Append(buffered[i].Throwable);
        }
        GD.PrintRich(_stringBuilder.ToString());
    }
}