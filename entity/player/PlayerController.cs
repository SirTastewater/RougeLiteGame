using Godot;

namespace RougeLiteGame.entity.player;

[GlobalClass]
public partial class PlayerController : EntityController
{
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