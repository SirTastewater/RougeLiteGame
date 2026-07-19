using Godot;
using RougeLiteGame.environment.rooms.room;
using System.Collections.Generic;

namespace RougeLiteGame.environment.rooms.corridor;

public partial class Corridor : Node3D
{
    private int _corridorLength = 20;
    private int X;
    private int Y;
    private DIRECTION _startDirection;
    private List<PackedScene> _corridorAssets = [];

    public Corridor(int x, int y, DIRECTION startDirection)
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/corridor/tunel.blend");
        _corridorAssets.Add(tmp);

        this._startDirection = startDirection;

        this.X = x;
        this.Y = y;
    }

    public void Rotate()
    {
        switch (_startDirection)
        {
            case DIRECTION.NORTH:
            case DIRECTION.SOUTH:
                this.GlobalRotationDegrees = new Vector3(0,90,0);
                break;
        }
    }

    public void UpdatePosition()
    {
        float offset = _corridorLength / 2 + (float)0.01;
        float yOffset = (float)-0.25;

        switch (_startDirection)
        {
            case DIRECTION.NORTH:
                this.GlobalPosition = new(this.X * _corridorLength, yOffset, this.Y * _corridorLength - offset);
                this.Scale =new Vector3(1,(float)0.99,1);
                break;
            case DIRECTION.SOUTH:
                this.GlobalPosition = new(this.X * _corridorLength, yOffset, this.Y * _corridorLength + offset);
                this.Scale =new Vector3(1,(float)0.99,1);
                break;
            case DIRECTION.WEST:
                this.GlobalPosition = new(this.X * _corridorLength - offset, yOffset, this.Y * _corridorLength);
                this.Scale =new Vector3((float)0.99,1,1);
                break;
            case DIRECTION.EAST:
                this.GlobalPosition = new(this.X * _corridorLength + offset, yOffset, this.Y * _corridorLength);
                this.Scale =new Vector3((float)0.99,1,1);
                break;
        }
        
    }

    public void Init()
    {
        Node tmp = _corridorAssets[0].Instantiate();
        this.AddChild(tmp);
    }
}