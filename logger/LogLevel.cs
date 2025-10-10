namespace RougeLiteGame.logger;

/// <summary>
/// Specifies the severity level of a log message.
/// </summary>
public enum LogLevel
{
    Trace,      // Most detailed
    Debug,      // General debug information
    Fine,       // Even more detail or sub-debug level
    Info,       // Routine information
    Success,    // Positive result (non-standard, but helpful)
    Warn,       // Something unexpected, but not an error
    Error,      // An error occurred, but the game can continue
    Critical,   // Serious failure could compromise stability
    Fatal       // System shutdown or unrecoverable error
}