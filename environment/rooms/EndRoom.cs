using Godot;
using RougeLiteGame.environment.dungeon;

namespace RougeLiteGame.environment.rooms;

public class EndRoom(Vector3I location, Vector3I from) : IRoom
{
    public Vector3I[] Location { get; } = [location];
    public int Size => 1;
    public int AssetId => 0;

    private Vector3I From => from;

    public int GetOrthogonalIndex(GridMap gridMap)
    {
        Direction direction = DirectionExtensions.GetDirectionByVectors(Location[0], From);
        return direction.GetOrthogonalIndex(gridMap);
    }
}