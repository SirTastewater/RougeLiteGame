using Godot;
using System.Collections.Generic;

namespace RougeLiteGame.environment.rooms.room;

public enum DIRECTION {NORTH, EAST, SOUTH, WEST, NONE}

public partial class Room : Node3D
{
    [Export] protected int _roomLength = 6;
    public int X;
    public int Y;
    public int Connections;
    public DIRECTION NextRoomDirection = DIRECTION.NONE;
    public DIRECTION LastRoomDirection = DIRECTION.NONE;
    public DIRECTION SideRoomDirection = DIRECTION.NONE;
    protected List<PackedScene> _roomAssets = [];

    public Room(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public Room()
    {
        
    }

    virtual public void Rotate()
    {
        
    }

    public void UpdatePosition()
    {
        this.Position = new(this.X * _roomLength, 0, this.Y * _roomLength);;
    }

    virtual public void Init()
    {

    }
}