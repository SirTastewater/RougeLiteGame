using System;
using Godot;

namespace RougeLiteGame.environment.dungeon;

public class PathStuckException(string message, Vector3I position) : Exception(message)
{
    public Vector3I Position => position;
    
    public PathStuckException(Vector3I position) : this("The path generator became stuck and could not continue.", position) { }
}