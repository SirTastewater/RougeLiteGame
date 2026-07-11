using Godot;

namespace RougeLiteGame.environment.rooms;

public interface IRoom
{
    Vector3I[] Location { get; }   
    
    int Size { get; }
    
    int AssetId { get; }

    int GetOrthogonalIndex(GridMap gridMap);
}