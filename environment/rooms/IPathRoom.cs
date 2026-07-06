using Godot;
using RougeLiteGame.environment.dungeon;

namespace RougeLiteGame.environment.rooms;

public interface IPathRoom : IRoom
{
    
    public Direction From { get; }
    public Direction To { get; }
    
}