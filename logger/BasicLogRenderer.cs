using System.Text;
using System.Threading;
using Godot;

namespace RougeLiteGame.logger;

public class BasicLogRenderer : ILogRenderer
{
    public virtual void Render(ref LogEntry logEntry)
    {
        StringBuilder stringBuilder = new(128);

        if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
        {
            stringBuilder.Append('[').Append(Thread.CurrentThread.Name).Append(']').Append(' ');
        }

        stringBuilder.Append('[').Append(Time.GetTimeStringFromSystem()).Append(']').Append(' ');
        stringBuilder.Append(logEntry.Level).Append(' ');
        stringBuilder.Append('[').Append(logEntry.Type).Append(']').Append(' ');
        stringBuilder.Append(logEntry.Message);

        logEntry.Message = stringBuilder.ToString();
    }

    public virtual void RenderArgument(ref string argument) { }
}