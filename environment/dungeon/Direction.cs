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
}