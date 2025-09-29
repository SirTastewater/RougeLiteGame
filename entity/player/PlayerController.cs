using Godot;
using RougeLiteGame.entity.camera;

namespace RougeLiteGame.entity.player;

[GlobalClass]
public partial class PlayerController : EntityController
{
    
    private CameraController _cameraController;
    [Export] private bool _lerpMovement;
    private Vector3 _cameraOffset;
    
    [Export] private CameraController CameraController
    {
        get => _cameraController;
        set
        {
            _cameraController = value;
            _isCameraInitialized = false;
        }
    }

    private bool _isCameraInitialized;

    public override void _PhysicsProcess(double delta)
    {
        
        base._PhysicsProcess(delta);
        // the code below must be executed after the base._PhysicsProcess(delta) as it relies on the Entity's position
        
        if (CameraController == null) return;

        if (!_isCameraInitialized)
        {
            SetupCameraController();
        }
        
        CameraController.GlobalPosition = Entity.GlobalPosition + _cameraOffset;
            
        Vector3 rotation = Entity.Rotation;
        float yRotation = CameraController.Yaw;
        if (_lerpMovement)
        {
            yRotation = Mathf.LerpAngle(rotation.Y, CameraController.Yaw, 0.1f);
        }
        
        Entity.Rotation = new Vector3(rotation.X, yRotation, rotation.Z);
    }

    private void SetupCameraController()
    {
        _cameraController.SetAsTopLevel(true);
        _cameraOffset = _cameraController.GlobalPosition - Entity.GlobalPosition;
        _isCameraInitialized = true;
    }

    protected override Vector3 MovementProcess(double delta)
    {
        float speed = MovementSpeed();
        Vector3 velocity = Entity.Velocity;
        
        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && Entity.IsOnFloor())
        {
            velocity.Y += JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
        Vector3 direction = (Entity.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * speed;
            velocity.Z = direction.Z * speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Entity.Velocity.X, 0, speed);
            velocity.Z = Mathf.MoveToward(Entity.Velocity.Z, 0, speed);
        }

        return velocity;
    }

    protected override bool IsSprinting()
    {
        return Input.IsActionPressed("sprint");
    }
}