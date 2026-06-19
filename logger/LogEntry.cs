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

    public void Interpolate(ILogRenderer renderer)
    {
        if (Arguments == null || Arguments.Length == 0) { return; }
        
        int argumentCount = Arguments.Length;
        
        if(Arguments?.LastOrDefault() is Exception exception)
        {
            Throwable = exception.StackTrace ?? exception.Message;
            argumentCount--;
        }

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
                    string message = (Arguments![argumentIndex++]?.ToString() ?? "null");
                    renderer.RenderArgument(ref message);
                    builder.Append(message);
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
        
    }
}