using System;
using Godot;
using RougeLiteGame.entity.camera;

namespace RougeLiteGame.entity.player;

/// <summary>
/// The PlayerController class manages player-specific logic for controlling movement.
/// </summary>
/// <remarks>
/// Inherits from <see cref="EntityController"/> to provide movement and state management.
/// Implements <see cref="ICameraActor"/> to act as a camera attachment point for the camera controller.
/// </remarks>
[GlobalClass] public partial class PlayerController : EntityController, ICameraActor
{
    #region Constants
    private const float Tolerance = 0.01f;
    #endregion
    
    #region Attribtutes
    private CameraController _cameraController;
    private Vector3 _relativeCameraPosition ;
    private bool _computedRelativeCameraPosition;
    #endregion
    
    [Export]
    private CameraController CameraController 
    { get => _cameraController; set { _cameraController = value;
        if (IsEntityConnected() && Entity.IsInsideTree()) { InitializeCamera(); }
    } }

    protected override void EntityReady()
    {
        InitializeCamera();
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

    public void CameraProcess(Node3D camera, float yaw, float pitch) // called every frame, be careful adding any code here
    {
        camera.GlobalPosition = Entity.GlobalPosition + _relativeCameraPosition;   
        
        Vector3 rotation = Entity.Rotation;
        if(FloatEquals(yaw, rotation.Y) && FloatEquals(pitch, rotation.X)) return;
        
        Entity.Rotation = new Vector3(rotation.X, yaw, rotation.Z);
    }

    private void InitializeCamera()
    {
        _cameraController.Actor = this;
        _relativeCameraPosition = _cameraController.GlobalPosition - Entity.GlobalPosition;
        _computedRelativeCameraPosition = true;
    }

    /// <summary>
    /// Determins whether two floating-point numbers differ by more than a predefined tolerance.
    /// </summary>
    /// <param name="a">The first float value to compare.</param>
    /// <param name="b">The second float value to compare.</param>
    /// <returns>
    /// <c>true</c> if the absolute difference between <paramref name="a"/> and <paramref name="b"/> 
    /// is greater than the allowed <c>Tolerance</c>; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is useful for comparing floating-point numbers where exact equality is unreliable 
    /// due to precision errors.
    /// </remarks>
    private static bool FloatEquals(float a, float b)
    {
        return Math.Abs(a - b) < Tolerance;
    }
}