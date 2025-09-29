using Godot;
using RougeLiteGame.entity.camera;

namespace RougeLiteGame.entity.player;

[GlobalClass]
public partial class FirstPersonPlayerController : PlayerController
{
    [Export]
    private CameraPivot _cameraPivot;
    
    private Vector3 _cameraOffset;
    
    public CameraPivot CameraPivot
    {
        get => _cameraPivot;
        set
        {
            _cameraPivot = value;
            _cameraOffset = _cameraPivot.GlobalPosition - Entity.GlobalPosition;
            _cameraPivot.OnCameraMoved = () =>
            {
                _cameraPivot.Position = Entity.Position;
            };
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (_cameraPivot == null) return;
        
        _cameraPivot.GlobalPosition = Entity.GlobalPosition + _cameraOffset;
            
        Vector3 rotation = Entity.Rotation;
        Entity.Rotation = new Vector3(rotation.X, _cameraPivot.Yaw, rotation.Z);
    }
}