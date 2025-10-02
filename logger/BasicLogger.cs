using System;
using System.Text.RegularExpressions;
using Godot;

namespace RougeLiteGame.logger;

public abstract partial class BasicLogger(Type type) : ILogger
{
    [GeneratedRegex(@"\{\}")] private static partial Regex Attribute();

    /// <summary>
    /// Logs a message with placeholders that are optionally replaced by provided parameter values.
    /// </summary>
    /// <param name="message">The message template that may include placeholders for parameter values.</param>
    /// <param name="parameters">An array of parameters to replace placeholders in the message template.</param>
    public void Log(object message, params object[] parameters)
    {
        Log(LogLevel.Info, message, parameters);
    }

    /// <summary>
    /// Logs a message with a specified severity level.
    /// The message may include placeholders that are replaced with provided parameter values.
    /// </summary>
    /// <param name="level">The severity level of the log message <see cref="LogLevel"/>.</param>
    /// <param name="message">The message template containing placeholders to be replaced with parameter values.</param>
    /// <param name="parameters">An array of parameters to replace placeholders in the message template. Optionally includes an exception as the last parameter.</param>
    public void Log(LogLevel level, object message, params object[] parameters)
    {
        /*if (!EngineDebugger.IsActive())
        { // TODO find better solution to not exclude rider runs
            return; // disable in production as logging is hilariously slow
        }*/
        
        string logMessage = message?.ToString();
        if(logMessage == null) return;
        
        if (parameters == null || parameters.Length == 0)
        {
            Out(level, logMessage);
            return;
        }

        if (parameters[^1] is Exception exception)
        {
            LogException(level, logMessage, parameters, exception);
            return;
        }

        int i = 0;
        Out(level, Attribute().Replace(logMessage, match =>
        {
            if (i < parameters.Length)
                return parameters[i++]?.ToString() ?? "null";
            return match.Value;
        }));
    }

    public void Trace(object message, params object[] parameters)
    {
        Log(LogLevel.Trace, message, parameters);
    }

    public void Fine(object message, params object[] parameters)
    {
        Log(LogLevel.Fine, message, parameters);
    }

    public void Debug(object message, params object[] parameters)
    {
        Log(LogLevel.Debug, message, parameters);
    }

    public void Info(object message, params object[] parameters)
    {
        Log(LogLevel.Info, message, parameters);
    }

    public void Success(object message, params object[] parameters)
    {
        Log(LogLevel.Success, message, parameters);
    }

    public void Error(object message, params object[] parameters)
    {
        Log(LogLevel.Error, message, parameters);
    }

    public void Warn(object message, params object[] parameters)
    {
        Log(LogLevel.Warn, message, parameters);
    }

    public void Critical(object message, params object[] parameters)
    {
        Log(LogLevel.Critical, message, parameters);
    }

    public void Fatal(object message, params object[] parameters)
    {
        Log(LogLevel.Fatal, message, parameters);
    }

    /// <summary>
    /// Logs a message along with a specified log level, including exception details.
    /// Formats the message using provided parameters or replaces placeholders appropriately.
    /// </summary>
    /// <param name="level">The severity level of the log message <see cref="LogLevel"/>.</param>
    /// <param name="message">The message template containing placeholders to be replaced with parameter values.</param>
    /// <param name="parameters">An array of parameters to replace placeholders in the message template. The last parameter may optionally be an exception.</param>
    /// <param name="throwable">An exception containing error details to include in the log output.</param>
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

    /// <summary>
    /// Outputs a log message to the desired destination (e.g., console, file, etc.)
    /// along with a specified log level. Optionally includes exception information.
    /// </summary>
    /// <param name="level">The severity level of the log message <see cref="LogLevel"/>.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="throwable">Optional. An exception object containing error details, if applicable.</param>
    protected abstract void Out(LogLevel level, string message, Exception throwable = null);
}