using Godot;
using RougeLiteGame.environment.dungeon;

namespace RougeLiteGame.environment.rooms;

public class StraightRoom(Vector3I location, Direction from, Direction to) : IPathRoom
{
    public Vector3I Location { get; } = location;
    public int Size => 1;
    public int AssetId => 1;

    public Direction From { get; } = from;

    public Direction To { get; } = to;

    public int GetOrthogonalIndex(GridMap gridMap)
    {
        return From.GetOrthogonalIndex(gridMap);
    }
}