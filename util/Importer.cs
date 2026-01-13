using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.util;

[Tool]
public partial class Importer : EditorScenePostImport
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger<Importer>();

    public override GodotObject _PostImport(Node scene)
    {
        return scene;
    }
}