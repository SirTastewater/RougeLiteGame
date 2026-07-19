using Godot;
using RougeLiteGame.environment.rooms.room;
using RougeLiteGame.environment.rooms.corridor;

namespace RougeLiteGame.environment.rooms.room_3;
public partial class FourDoorRoom : Room
{
    public FourDoorRoom(int x, int y) : base(x,y)
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_4/main_path_four_door_room.blend");
        this._roomAssets.Add(tmp);
        this.Connections = 4;
    }

    public FourDoorRoom() : base()
    {
        PackedScene tmp = GD.Load<PackedScene>("res://environment/rooms/room_4/main_path_four_door_room.blend");
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

        Corridor tmpCorridor = new Corridor(this.X,this.Y,this.NextRoomDirection);

        this.AddChild(tmpCorridor);

        tmpCorridor.Init();

        tmpCorridor.Rotate();

        tmpCorridor.UpdatePosition();

        tmpCorridor = new Corridor(this.X,this.Y,this.SideRoomDirection);

        this.AddChild(tmpCorridor);

        tmpCorridor.Init();

        tmpCorridor.Rotate();

        tmpCorridor.UpdatePosition();

        DIRECTION missingDirection = DIRECTION.NONE;
        switch (this.LastRoomDirection)
        {
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.SOUTH && SideRoomDirection == DIRECTION.EAST:
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.EAST && SideRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.NORTH && SideRoomDirection == DIRECTION.EAST:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.EAST && SideRoomDirection == DIRECTION.NORTH:
            case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.SOUTH && SideRoomDirection == DIRECTION.NORTH:
            case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.NORTH && SideRoomDirection == DIRECTION.SOUTH:
                missingDirection = DIRECTION.WEST;
                break;
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.WEST && SideRoomDirection == DIRECTION.EAST:
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.EAST && SideRoomDirection == DIRECTION.WEST:
            case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.NORTH && SideRoomDirection == DIRECTION.EAST:
            case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.EAST && SideRoomDirection == DIRECTION.NORTH:
            case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.WEST && SideRoomDirection == DIRECTION.NORTH:
            case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.NORTH && SideRoomDirection == DIRECTION.WEST:
                missingDirection = DIRECTION.SOUTH;
                break;
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.WEST && SideRoomDirection == DIRECTION.EAST:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.EAST && SideRoomDirection == DIRECTION.WEST:
            case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.SOUTH && SideRoomDirection == DIRECTION.EAST:
            case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.EAST && SideRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.WEST && SideRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.EAST when this.NextRoomDirection == DIRECTION.SOUTH && SideRoomDirection == DIRECTION.WEST:
                missingDirection = DIRECTION.NORTH;
                break;
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.SOUTH && SideRoomDirection == DIRECTION.WEST:
            case DIRECTION.NORTH when this.NextRoomDirection == DIRECTION.WEST && SideRoomDirection == DIRECTION.SOUTH:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.NORTH && SideRoomDirection == DIRECTION.WEST:
            case DIRECTION.SOUTH when this.NextRoomDirection == DIRECTION.WEST && SideRoomDirection == DIRECTION.NORTH:
            case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.SOUTH && SideRoomDirection == DIRECTION.NORTH:
            case DIRECTION.WEST when this.NextRoomDirection == DIRECTION.NORTH && SideRoomDirection == DIRECTION.SOUTH:
                missingDirection = DIRECTION.EAST;
                break;
        }

        tmpCorridor = new Corridor(this.X,this.Y,missingDirection);

        this.AddChild(tmpCorridor);

        tmpCorridor.Init();

        tmpCorridor.Rotate();

        tmpCorridor.UpdatePosition();
    }
}