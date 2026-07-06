using Godot;
using Godot.Collections;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment.rooms;

[Tool]
public partial class RoomLibraryGenerator : EditorScenePostImport
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger<RoomLibraryGenerator>(false);
    
    public override GodotObject _PostImport(Node scene)
    {
        MeshLibrary library = new MeshLibrary();

        Logger.Info("Start scanning resource {} for assets", scene.Name);
        Array<Node> children = scene.GetChildren();
        for (int i = 0; i < children.Count; i++)
        {
            RegisterEntry(i, children[i], library);
        }

        const string path = "res://environment/rooms/RoomLibrary.meshlib";
        Logger.Info("Save library file in path {}.", path);
        ResourceSaver.Save(library, path);
        
        Logger.Flush();

        return scene;
    }

    private static void RegisterEntry(int index, Node child, MeshLibrary library)
    {
        Logger.Trace("Register {} as asset in the mesh library", child.Name);
        if (child is not MeshInstance3D mesh || mesh.Mesh == null) { return; }
        library.CreateItem(index);
        library.SetItemMesh(index, mesh.GetMesh());
        library.SetItemName(index, $"[{index}] {child.Name}");
        
        mesh.CreateTrimeshCollision(); // generate collision shape
        
        StaticBody3D staticBody3D = mesh.GetChild(0) as StaticBody3D;
        if (staticBody3D?.GetChild(0) is not CollisionShape3D shape) return;
        
        Logger.Trace("Adding shape {} to asset {}.", shape.Shape, child.Name);
        library.SetItemShapes(index, [shape.Shape]);
    }
}