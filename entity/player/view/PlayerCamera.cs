using Godot;

namespace RougeLiteGame.entity.player.view;

[GlobalClass] public partial class PlayerCamera : Node3D
{
    [Export] public float MouseSensitivity { get; private set; } = 0.01f;
    public float Yaw { get; set; }
    public float Pitch { get; set; }

}