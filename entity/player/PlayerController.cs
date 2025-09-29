using Godot;

namespace RougeLiteGame.entity.player;

[GlobalClass]
public partial class PlayerController : EntityController
{
    //[Export] private view.PlayerCamera _viewCamera;
    
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

    /*public override void _Input(InputEvent @event)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        if (@event is not InputEventMouseMotion motionEvent) return;

        _viewCamera.Yaw -= motionEvent.Relative.X * _viewCamera.MouseSensitivity;
        _viewCamera.Pitch -= motionEvent.Relative.Y * _viewCamera.MouseSensitivity;
        _viewCamera.Pitch = Mathf.Clamp(_viewCamera.Pitch, -1.4f, 1.4f);

        Entity.Rotation = new Vector3(Entity.Rotation.X, _viewCamera.Yaw, Entity.Rotation.Z);

        _viewCamera.Rotation = new Vector3(_viewCamera.Pitch, _viewCamera.Rotation.Y, _viewCamera.Rotation.Z);
        base._Input(@event);
    }*/

    protected override bool IsSprinting()
    {
        return Input.IsActionPressed("sprint");
    }
}