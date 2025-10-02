using System;
using System.Text.RegularExpressions;

namespace RougeLiteGame.logger;

public abstract partial class BasicLogger(Type type) : ILogger
{
    [GeneratedRegex(@"\{\}")] private static partial Regex Attribute();

    public void Log(string message, params object[] parameters)
    {
        Log(LogLevel.INFO, message, parameters);
    }

    public void Log(LogLevel level, string message, params object[] parameters)
    {
        if (parameters == null || parameters.Length == 0)
        {
            Out(level, message);
            return;
        }

        if (parameters[^1] is Exception exception)
        {
            LogException(level, message, parameters, exception);
            return;
        }

        int i = 0;
        Out(level, Attribute().Replace(message, match =>
        {
            if (i < parameters.Length)
                return parameters[i++]?.ToString() ?? "null";
            return match.Value;
        }));
    }

    private void LogException(LogLevel level, string message, object[] parameters, Exception throwable)
    {
        int placeholderCount = Attribute().Matches(message).Count;

        if (parameters.Length == 1 && placeholderCount == 1)
        {
            Out(level, message.Replace("{}", throwable.ToString()), throwable);
            return;
        }
        
        // Remove throwable from the parameters. (Yes throwable is a java word. Imma just use it!)
        Array.Resize(ref parameters, parameters.Length - 1);
        int index = 0;
        Out(level, Attribute().Replace(message, match =>
        {
            if (index < parameters.Length)
            {
                return parameters[index++]?.ToString() ?? "null";
            }

            return match.Value;
        }), throwable);
        
    }

    protected abstract void Out(LogLevel level, string message, Exception throwable = null);
}