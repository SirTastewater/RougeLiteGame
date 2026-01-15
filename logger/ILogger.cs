using System;

namespace RougeLiteGame.logger;

/// <summary>
///     Provides a contract for logging messages with varying levels of severity.
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     Logs a formatted message at the <c>INFO</c> level, optionally replacing placeholders in the message with provided
    ///     parameter values.
    /// </summary>
    /// <param name="message">
    ///     A message template containing zero or more <c>{}</c> placeholders to be replaced with the corresponding values in
    ///     <paramref name="parameters" />.
    /// </param>
    /// <param name="parameters">
    ///     An optional array of parameter values to inject into the message template. If the last parameter is an
    ///     <see cref="Exception" />, it will be handled separately and included in the log entry appropriately.
    /// </param>
    /// <remarks>
    ///     This is a convenience method that delegates to <see cref="Log(LogLevel, string, object[])" /> using
    ///     <see cref="LogLevel.Info" /> as the default log level.
    ///     <para>
    ///         Placeholder replacement is performed in the order parameters are provided. If fewer parameters are supplied
    ///         than placeholders, unmatched placeholders will remain in the output.
    ///         If more parameters are supplied than placeholders, the excess parameters will be ignored, except in the case of
    ///         an <see cref="Exception" /> as the last argument, which is handled specially.
    ///     </para>
    ///     <para>
    ///         When an <see cref="Exception" /> is detected as the final parameter:
    ///         <list type="bullet">
    ///             <item>
    ///                 If there is only one placeholder and one parameter (the exception), the placeholder is replaced with
    ///                 the exception’s <c>ToString()</c> output.
    ///             </item>
    ///             <item>
    ///                 Otherwise, the exception is removed from the parameter list and appended to the log output using
    ///                 structured exception logging.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    // This was written by AI, but it was too good to not be used
    void Log(object message, params object[] parameters);

    /// <summary>
    ///     Logs a message with a specified severity level.
    ///     The message may include placeholders that are replaced with provided parameter values.
    /// </summary>
    /// <param name="level">The severity level of the log message.</param>
    /// <param name="message">The message template containing placeholders to be replaced with parameter values.</param>
    /// <param name="parameters">
    ///     An array of parameters to replace placeholders in the message template. Optionally includes an
    ///     exception as the last parameter.
    /// </param>
    void Log(LogLevel level, object message, params object[] parameters);
    
    void Log(LogLevel level, string message, params object[] parameters);

    // you are allowed to write the doc for me (:

    void Trace(object message, params object[] parameters);
    
    void Trace(string message, params object[] parameters);

    void Fine(object message, params object[] parameters);
    
    void Fine(string message, params object[] parameters);

    void Debug(object message, params object[] parameters);
    
    void Debug(string message, params object[] parameters);

    void Info(object message, params object[] parameters);
    
    void Info(string message, params object[] parameters);

    void Success(object message, params object[] parameters);
    
    void Success(string message, params object[] parameters);

    void Error(object message, params object[] parameters);
    
    void Error(string message, params object[] parameters);

    void Warn(object message, params object[] parameters);
    
    void Warn(string message, params object[] parameters);

    void Critical(object message, params object[] parameters);
    
    void Critical(string message, params object[] parameters);

    void Fatal(object message, params object[] parameters);
    
    void Fatal(string message, params object[] parameters);

    public void Flush();

    public static string LogLevelToColor(LogLevel level)
    {
        // inspired by LTRO-1 Palette
        // https://lospec.com/palette-list/ltro-1
        // slightly modified

        return level switch
        {
            LogLevel.Trace => "#7d7781", // dark-grey 
            LogLevel.Debug => "#4159cb", // blue
            LogLevel.Fine => "#59a7af", // cyan
            LogLevel.Info => "#b8afbe", // light gray
            LogLevel.Success => "#8d902e", // green
            LogLevel.Warn => "#fdbb27", // yellow
            LogLevel.Error => "#997171", // lighter gray-redish
            LogLevel.Critical => "#89423f", // matt red
            LogLevel.Fatal => "#f63f4c", // burning eyes red
            _ => "#FFFFFF"
        };
    }
}