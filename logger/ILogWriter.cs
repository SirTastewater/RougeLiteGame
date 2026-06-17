namespace RougeLiteGame.logger;

public interface ILogWriter
{
    public void Write(LogEntry[] buffered);
}