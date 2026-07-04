using Godot;
using RougeLiteGame.environment.rooms.room;

namespace RougeLiteGame.environment.rooms.room_1;
public partial class OneDoorRoom : Room
{
    public OneDoorRoom(int x, int y) : base(x,y)
    {
		PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_1/room_1.blend");
        this._roomAssets.Add(tmp);
        Connections = 1;
    }

    public OneDoorRoom() : base()
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_1/room_1.blend");
        this._roomAssets.Add(tmp);
        Connections = 1;
    }

    override public void Rotate() {
        Vector3 rotationVector = new(0,0,0);

		switch (this.NextRoomDirection)
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

		this.RotationDegrees = rotationVector;
    }

    public override void Init()
    {
        Node tmp = this._roomAssets[0].Instantiate();
		this.AddChild(tmp);
    }
}