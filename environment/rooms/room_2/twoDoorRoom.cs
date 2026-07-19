using System.Collections.Generic;
using Godot;
using RougeLiteGame.environment.rooms.room;
using RougeLiteGame.environment.rooms.corridor;

namespace RougeLiteGame.environment.rooms.room_2;
public partial class TwoDoorRoom : Room
{
    private List<PackedScene> _curvedRoomAssets = [];
    private List<PackedScene> _straightRoomAssets = [];
    private bool _isStraight;
    private bool _isOnSidePath;
    private List<PackedScene> _sidePathAssetsStraight = [];
    private List<PackedScene> _sidePathAssetsCurved = [];
    public TwoDoorRoom(int x, int y, bool isOnSidePath = false) : base(x,y)
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/main_path_two_door_room_curved.blend");
        this._curvedRoomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/side_path_two_door_room_curved.blend");
        this._sidePathAssetsCurved.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/main_path_two_door_room_straight.blend");
        this._straightRoomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/side_path_two_door_room_straight.blend");
        this._sidePathAssetsStraight.Add(tmp);

        this.Connections = 2;

        this._isOnSidePath = isOnSidePath;
    }

    public TwoDoorRoom() : base()
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/main_path_two_door_room_curved.blend");
        this._curvedRoomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/side_path_two_door_room_curved.blend");
        this._sidePathAssetsCurved.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/main_path_two_door_room_straight.blend");
        this._straightRoomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/side_path_two_door_room_straight.blend");
        this._sidePathAssetsStraight.Add(tmp);

        this.Connections = 2;
    }

    public override void Rotate()
    {
        if (this._isStraight)
        {
            RotateStraightRoom();
        }
        else
        {
            RotateCurvedRoom();
        }
    }

    private void RotateStraightRoom()
	{
		Vector3 rotationVector = new(0,0,0);

		switch (this.NextRoomDirection)
		{
			case DIRECTION.EAST:
			case DIRECTION.WEST:
				rotationVector.Y += 90;
				break;
			case DIRECTION.SOUTH:
			case DIRECTION.NORTH:
				rotationVector.Y += 0;
				break;
		}

		this.GlobalRotationDegrees = rotationVector;
	}

	private void RotateCurvedRoom()
	{
		Vector3 rotationVector = new(0,0,0);

		switch (this.LastRoomDirection)
		{
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.NORTH:
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.EAST:
				rotationVector.Y += 90;
				break;
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.EAST:
				rotationVector.Y += 0;
				break;
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.NORTH:
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.WEST:
				rotationVector.Y += 180;
				break;
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.WEST:
				rotationVector.Y += 270;
				break;
		}

		this.GlobalRotationDegrees = rotationVector;
	}

    public override void Init()
    {
        switch (this.LastRoomDirection)
		{
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.WEST:
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.EAST:
			case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.SOUTH:
			case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.NORTH:
				this._isStraight = true;
                break;
            default:
				this._isStraight = false;
				break;
		}

        if (this._isStraight)
        {
            if (this._isOnSidePath)
            {
                Node tmp = this._sidePathAssetsStraight[0].Instantiate();
                this.AddChild(tmp);
            }
            else
            {
                Node tmp = this._straightRoomAssets[0].Instantiate();
                this.AddChild(tmp);
            }
        }
        else
        {
            if (this._isOnSidePath)
            {
                Node tmp = this._sidePathAssetsCurved[0].Instantiate();
                this.AddChild(tmp);
            }
            else
            {
                Node tmp = this._curvedRoomAssets[0].Instantiate();
                this.AddChild(tmp);
            }
        }

        Corridor tmpCorridor = new Corridor(this.X,this.Y,this.NextRoomDirection);

        this.AddChild(tmpCorridor);

        tmpCorridor.Init();

        tmpCorridor.Rotate();

        tmpCorridor.UpdatePosition();
    }
}