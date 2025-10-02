namespace RougeLiteGame.logger;

public interface ILogger
{
    void Log(string message, params object[] parameters);
    
    void Log(LogLevel level, string message, params object[] parameters);
}