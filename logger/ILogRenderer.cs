namespace RougeLiteGame.logger;

public interface ILogRenderer
{
    public void Render(ref LogEntry logEntry);
}