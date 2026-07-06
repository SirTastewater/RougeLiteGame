using System;

namespace RougeLiteGame.environment.dungeon;

public class PathStuckException(string message) : Exception(message)
{
    public PathStuckException() : this("The path generator became stuck and could not continue.") { }
}