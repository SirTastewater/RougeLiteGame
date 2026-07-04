using Godot;
using RougeLiteGame.environment.rooms.room;

namespace RougeLiteGame.environment.rooms.room_3;
public partial class ThreeDoorRoom : Room
{
    private bool _isStraight;
    public ThreeDoorRoom(int x, int y) : base(x,y)
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_3/room_3.blend");
        this._roomAssets.Add(tmp);
        Connections = 3;
    }

    public ThreeDoorRoom() : base()
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_3/room_3.blend");
        this._roomAssets.Add(tmp);
        Connections = 3;
    }

    public override void Rotate()
    {
        if (this._isStraight)
        {
            RotateStraightPathRoom();
        }
        else
        {
            RotateCurvedPathRoom();
        }
	}

    private void RotateStraightPathRoom()
	{
		Vector3 rotationVector = new(0,0,0);

		switch (this.NextRoomDirection)
		{
			case DIRECTION.EAST:
			case DIRECTION.WEST:
				rotationVector.Y += 0;
				break;
			case DIRECTION.SOUTH:
			case DIRECTION.NORTH:
				rotationVector.Y += 90;
				break;
		}

		this.RotationDegrees = rotationVector;
	}

    private void RotateCurvedPathRoom()
    {
        Vector3 rotationVector = new(0,0,0);

		switch (this.LastRoomDirection)
		{
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.NORTH && this.SideRoomDirection == DIRECTION.WEST:
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.WEST && this.SideRoomDirection == DIRECTION.NORTH:
			case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.EAST && this.SideRoomDirection == DIRECTION.WEST:
			case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.WEST && this.SideRoomDirection == DIRECTION.EAST:
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.NORTH && this.SideRoomDirection == DIRECTION.EAST:
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.EAST && this.SideRoomDirection == DIRECTION.NORTH:
				rotationVector.Y += 180;
				break;
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.NORTH && this.SideRoomDirection == DIRECTION.SOUTH:
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.SOUTH && this.SideRoomDirection == DIRECTION.NORTH:
			case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.EAST && this.SideRoomDirection == DIRECTION.SOUTH:
			case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.SOUTH && this.SideRoomDirection == DIRECTION.EAST:
			case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.NORTH && this.SideRoomDirection == DIRECTION.EAST:
			case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.EAST && this.SideRoomDirection == DIRECTION.NORTH:
				rotationVector.Y += 90;
				break;
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.NORTH && this.SideRoomDirection == DIRECTION.SOUTH:
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.SOUTH && this.SideRoomDirection == DIRECTION.NORTH:
			case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.WEST && this.SideRoomDirection == DIRECTION.SOUTH:
			case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.SOUTH && this.SideRoomDirection == DIRECTION.WEST:
			case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.NORTH && this.SideRoomDirection == DIRECTION.WEST:
			case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.WEST && this.SideRoomDirection == DIRECTION.NORTH:
				rotationVector.Y += 270;
				break;
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.EAST && this.SideRoomDirection == DIRECTION.SOUTH:
			case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.SOUTH && this.SideRoomDirection == DIRECTION.EAST:
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.WEST && this.SideRoomDirection == DIRECTION.SOUTH:
			case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.SOUTH && this.SideRoomDirection == DIRECTION.WEST:
			case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.EAST && this.SideRoomDirection == DIRECTION.WEST:
			case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.WEST && this.SideRoomDirection == DIRECTION.EAST:
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

		Node tmp = this._roomAssets[0].Instantiate();
		this.AddChild(tmp);
    }
}