using Godot;
using RougeLiteGame.environment.dungeon;

namespace RougeLiteGame.environment.rooms;

public class CurvedRoom(Vector3I location, Direction from, Direction to) : IPathRoom
{
    public Vector3I[] Location { get; } = [location];
    public int Size => 1;
    public int AssetId => 2;

    public Direction From { get; } = from;

    public Direction To { get; } = to;
    
    public int GetOrthogonalIndex(GridMap gridMap)
    {
        Direction direction = GetOrientationDirection();
        return direction.GetOrthogonalIndex(gridMap);
    }

    private Direction GetOrientationDirection() => (From, To) switch
    { // I hate it as much as you do, alright ?
        
        // if it should go from east to south it must rotate once
        (Direction.East,  Direction.South) or (Direction.South, Direction.East) => Direction.East,
        
        // if it is from south to west it must rotate two times
        (Direction.South, Direction.West) or (Direction.West,  Direction.South) => Direction.South,
        
        // if it should go from north to west it must rotate three times
        (Direction.North, Direction.West) or (Direction.West,  Direction.North) => Direction.West,
        
        // it's default orientation is from north to east
        _ => Direction.North
    };
}