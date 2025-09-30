using Godot;
using RougeLiteGame.entity.camera;

namespace RougeLiteGame.entity.player;

/// <summary>
/// The PlayerController class manages player-specific logic for controlling movement.
/// </summary>
/// <remarks>
/// Inherits from <see cref="EntityController"/> to provide movement and state management.
/// </remarks>
[GlobalClass] public partial class PlayerController : EntityController
{
    #region Constants
    private const float Tolerance = 0.01f;
    #endregion
    
    #region Attribtutes

    private Vector3 _relativeCameraPosition ;
    private bool _computedRelativeCameraPosition;
    #endregion

    [Export] private CameraController CameraController { get; set; }

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