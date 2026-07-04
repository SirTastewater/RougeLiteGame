using Godot;
using RougeLiteGame.environment.rooms.room;

namespace RougeLiteGame.environment.rooms.room_3;
public partial class FourDoorRoom : Room
{
    public FourDoorRoom(int x, int y) : base(x,y)
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_4/room_4.blend");
        this._roomAssets.Add(tmp);
        this.Connections = 4;
    }

    public FourDoorRoom() : base()
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_4/room_4.blend");
        this._roomAssets.Add(tmp);
        this.Connections = 4;
    }

    public override void Rotate()
    {
        
    }

    public override void Init()
    {
        Node tmp = this._roomAssets[0].Instantiate();
        this.AddChild(tmp);
    }
}