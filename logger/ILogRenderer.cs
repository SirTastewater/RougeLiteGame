namespace RougeLiteGame.logger;

public interface ILogRenderer
{
    public void Render(ref LogEntry logEntry);
    
    public void RenderArgument(ref string argument);
}