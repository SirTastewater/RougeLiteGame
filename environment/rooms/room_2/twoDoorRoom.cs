using System.Collections.Generic;
using Godot;
using RougeLiteGame.environment.rooms.room;

namespace RougeLiteGame.environment.rooms.room_2;
public partial class TwoDoorRoom : Room
{
    private List<PackedScene> _curvedRoomAssets = [];
    private List<PackedScene> _straightRoomAssets = [];
    private bool _isStraight;
    public TwoDoorRoom(int x, int y) : base(x,y)
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/room_2_curve.blend");
        this._curvedRoomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("res://environment/rooms/room_2/room_2_straight.blend");
        this._straightRoomAssets.Add(tmp);

        this.Connections = 2;
    }

    public TwoDoorRoom() : base()
    {
        PackedScene tmp = GD.Load<PackedScene>("environment\\rooms\\room_2\\room_2_curve.blend");
        this._curvedRoomAssets.Add(tmp);

        tmp = GD.Load<PackedScene>("environment\\rooms\\room_2\\room_2_straight.blend");
        this._straightRoomAssets.Add(tmp);

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

		this.RotationDegrees = rotationVector;
	}

	private void RotateCurvedRoom()
	{
		Vector3 rotationVector = new(0,0,0);

		switch (this.LastRoomDirection)
		{
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.NORTH:
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.EAST:
				rotationVector.Y += 180;
				break;
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.EAST:
				rotationVector.Y += 90;
				break;
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.NORTH:
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.WEST:
				rotationVector.Y += 270;
				break;
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.WEST:
				rotationVector.Y += 0;
				break;
		}

		this.RotationDegrees = rotationVector;
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
		}

		this._isStraight = false;

        if (this._isStraight)
        {
            Node tmp = this._straightRoomAssets[0].Instantiate();
            this.AddChild(tmp);
        }
        else
        {
            Node tmp = this._curvedRoomAssets[0].Instantiate();
            this.AddChild(tmp);
        }
    }

}