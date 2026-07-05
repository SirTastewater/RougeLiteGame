using System.Collections.Generic;
using Godot;
using RougeLiteGame.environment.rooms.room;

namespace RougeLiteGame.environment.rooms.room_1;
public partial class OneDoorRoom : Room
{
    private bool _isOnSidePath;
    private List<PackedScene> _sidePathAssets = [];
    public OneDoorRoom(int x, int y, bool isOnSidePath = false) : base(x,y)
    {
		PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_1/main_path_one_door_room.blend");
        this._roomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_1/side_path_one_door_room.blend");
        this._sidePathAssets.Add(tmp);

        Connections = 1;

        this._isOnSidePath = isOnSidePath;
    }

    public OneDoorRoom() : base()
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_1/main_path_one_door_room.blend");
        this._roomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_1/side_path_one_door_room.blend");
        this._roomAssets.Add(tmp);

        Connections = 1;
    }

    override public void Rotate() {
        Vector3 rotationVector = new(0,0,0);

        DIRECTION rotationDirection = DIRECTION.NONE;

        if(NextRoomDirection == DIRECTION.NONE)
        {
            rotationDirection = LastRoomDirection;
        }
        else
        {
            rotationDirection = NextRoomDirection;
        }

		switch (rotationDirection)
		{
			case DIRECTION.EAST:
				rotationVector.Y += 90;
				break;
			case DIRECTION.WEST:
				rotationVector.Y += 270;
				break;
			case DIRECTION.SOUTH:
				break;
			case DIRECTION.NORTH:
				rotationVector.Y += 180;
				break;
		}

		this.GlobalRotationDegrees = rotationVector;
    }

    public override void Init()
    {
        Node tmp = null;

        if (_isOnSidePath)
        {
            tmp = this._sidePathAssets[0].Instantiate();
        }
        else
        {
            tmp = this._roomAssets[0].Instantiate();
        }

		this.AddChild(tmp);
    }
}