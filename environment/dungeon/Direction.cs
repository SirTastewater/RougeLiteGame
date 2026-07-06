using System;
using Godot;

namespace RougeLiteGame.environment.dungeon;

public enum Direction
{
    North,
    East,
    South,
    West
}

public static class DirectionExtensions
{
    public static Vector2I ToVector(this Direction direction) => direction switch
    {
        Direction.North => new Vector2I(0, -1),
        Direction.East  => new Vector2I(1, 0),
        Direction.South => new Vector2I(0, 1),
        Direction.West  => new Vector2I(-1, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    public static Vector3I ToRotation(this Direction direction) => direction switch
    {
        Direction.North => Vector3I.Forward,
        Direction.East  => Vector3I.Right,
        Direction.South => Vector3I.Back,
        Direction.West  => Vector3I.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    public static bool IsOppositeOf(this Direction direction, Direction other) => direction switch
    {
        Direction.North when other == Direction.South => true,
        Direction.South when other == Direction.North => true,
        Direction.East when other == Direction.West => true,
        Direction.West when other == Direction.East => true,
        _ => false
    };
    
    public static int GetOrthogonalIndex(this Direction direction, GridMap gridMap) 
    {
        Vector3I vector3I = direction.ToRotation();
        Basis basis = Basis.LookingAt(vector3I, Vector3I.Up);
        return gridMap.GetOrthogonalIndexFromBasis(basis);
    }
    
    public static Direction GetDirectionByVectors(Vector3I from, Vector3I to)
    {
        Vector3I result = (to - from).Sign();
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            Vector2I vector2I = dir.ToVector();
            if (vector2I.X == result.X && vector2I.Y == result.Z)
            {
                return dir;
            }
        }

        return Direction.East;
    }
}