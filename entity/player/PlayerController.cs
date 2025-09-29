using Godot;
using RougeLiteGame.entity.camera;

namespace RougeLiteGame.entity.player;

[GlobalClass] public partial class PlayerController : EntityController
{
    #region Attribtutes
    private CameraController _cameraController;
    private Vector3 _cameraOffset;
    private bool _isCameraInitialized;
    #endregion
    
    #region Camera Settings
    [Export]
    private CameraController CameraController
    {
        get => _cameraController;
        set
        {
            _cameraController = value;
            _isCameraInitialized = false;
        }
    }
    
    // this also is a preparation for implementing a third-person camera
    // Lerping in first person is not a good idea 
    [Export] private bool _lerpMovement; 
    #endregion

    /**
     * Utility method to set up the camera controller.
     * It will set the camera controller as a top-level node and will
     * compute the offset between the camera and the entity to correctly move the camera with the entity.
     *
     * I've done it this way, because if we decide to also add third-person camera, the entities rotation won't be
     * attached to the camera.
     */
    private void SetupCameraController()
    {
        _cameraController.SetAsTopLevel(true);
        _cameraOffset = _cameraController.GlobalPosition - Entity.GlobalPosition;
        _isCameraInitialized = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        // the code below must be executed after the base._PhysicsProcess(delta) as it relies on the Entity's position

        if (CameraController == null) return;

        // TODO: we might need to move this out the physics process to save that computation 
        if (!_isCameraInitialized) 
        {
            SetupCameraController();
        }
        
        CameraController.GlobalPosition = Entity.GlobalPosition + _cameraOffset;

        Vector3 rotation = Entity.Rotation;
        float yRotation = CameraController.Yaw;
        if (_lerpMovement) yRotation = Mathf.LerpAngle(rotation.Y, CameraController.Yaw, 0.1f);

        Entity.Rotation = new Vector3(rotation.X, yRotation, rotation.Z);
    }

    protected override Vector3 MovementProcess(double delta)
    {
        float speed = MovementSpeed();
        Vector3 velocity = Entity.Velocity;

        if (Input.IsActionJustPressed("jump") && Entity.IsOnFloor()) velocity.Y += JumpVelocity;
        
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

    protected override bool IsSneaking()
    {
        return Input.IsActionPressed("sneak");
    }

    protected override bool IsSprinting()
    {
        return Input.IsActionPressed("sprint");
    }
}