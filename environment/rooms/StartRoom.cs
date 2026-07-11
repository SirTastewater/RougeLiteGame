using Godot;
using RougeLiteGame.environment.dungeon;

namespace RougeLiteGame.environment.rooms;

public class StartRoom(Vector3I location, Vector3I to) : IRoom
{
    public Vector3I[] Location { get; } = [location];
    public int Size => 1;
    public int AssetId => 0;

    private Vector3I To => to;

    public int GetOrthogonalIndex(GridMap gridMap)
    {
        Direction direction = DirectionExtensions.GetDirectionByVectors(Location[0], To);
        return direction.GetOrthogonalIndex(gridMap);
    }
}