namespace RougeLiteGame.logger;

public class ConsoleLogRenderer : BasicLogRenderer
{
    private static readonly string ArgumentColor = ILogger.LogLevelToColor(LogLevel.Argument);
    
    public override void RenderArgument(ref string argument)
    {
        argument = "[color=" + ArgumentColor + "]" + argument + "[/color]";
    }
}