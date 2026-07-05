using Godot;
using Godot.Collections;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment.rooms;

[Tool]
public partial class RoomLibraryGenerator : EditorScenePostImport
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger<RoomLibraryGenerator>();
    
    public override GodotObject _PostImport(Node scene)
    {
        MeshLibrary library = new MeshLibrary();

        Logger.Info("Start scanning resource {} for assets", scene.Name);
        Array<Node> children = scene.GetChildren();
        for (int i = 0; i < children.Count; i++)
        {
            RegisterEntry(i, children[i], library);
        }

        const string path = "environment/rooms/RoomLibrary.meshlib";
        Logger.Info("Save library file in path {}.", path);
        ResourceSaver.Save(library, path, ResourceSaver.SaverFlags.RelativePaths);

        return scene;
    }

    private static void RegisterEntry(int index, Node child, MeshLibrary library)
    {
        Logger.Trace("Register {} as asset in the mesh library", child.Name);
        library.CreateItem(index);
        if (child is not MeshInstance3D meshInstance) { return; }
        library.SetItemMesh(index, meshInstance.GetMesh());
        library.SetItemName(index, $"[{index}] {child.Name}");
            
        StaticBody3D staticBody3D = meshInstance.GetChild(0) as StaticBody3D;
        if (staticBody3D?.GetChild(0) is not CollisionShape3D shape) return;
        
        Logger.Trace("Adding shape to asset {}.", child.Name);
        library.SetItemShapes(index, [shape]);
    }
}