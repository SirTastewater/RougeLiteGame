using System;
using System.Linq;
using System.Text;
using System.Threading;
using Godot;

namespace RougeLiteGame.logger;

public struct LogEntry
{
    public LogLevel Level;
    public string Message;
    public object[] Arguments;
    public string Throwable; // lazily set. Do not trust checking
    public string Type;

    public void Interpolate()
    {
        if (Arguments == null || Arguments.Length == 0) { return; }
        
        int argumentCount = Arguments.Length;
        
        Throwable = (Arguments?.LastOrDefault() as Exception)?.StackTrace;
        if (Throwable != null) argumentCount--;

        int argumentIndex = 0;
        StringBuilder builder = new(Message.Length + 32);

        for (int i = 0; i < Message.Length; i++)
        {
            if (i + 1 >= Message.Length)
            {
                builder.Append(Message[i]);
                break;
            }

            if (Message[i] == '{' && Message[i + 1] == '}')
            {
                if (argumentIndex < argumentCount)
                {
                    builder.Append(Arguments![argumentIndex++]?.ToString() ?? "null");
                    i++;
                    continue;
                }

                builder.Append("{}");
                i++;
                continue;
            }

            builder.Append(Message[i]);
        }

        Message = builder.ToString();
    }
    
    public void Render()
    {
        StringBuilder stringBuilder = new(128);

        if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
        {
            stringBuilder.Append('[').Append(Thread.CurrentThread.Name).Append(']').Append(' ');
        }

        stringBuilder.Append('[').Append(Time.GetTimeStringFromSystem()).Append(']').Append(' ');
        stringBuilder.Append(Level).Append(' ');
        stringBuilder.Append('[').Append(Type).Append(']').Append(' ');
        stringBuilder.Append(Message);

        Message = stringBuilder.ToString();
    }
}