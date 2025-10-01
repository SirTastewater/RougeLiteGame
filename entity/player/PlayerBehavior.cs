using Godot;
using RougeLiteGame.entity.behavior;

namespace RougeLiteGame.entity.player;

[GlobalClass]
public partial class PlayerBehavior : IdleBehavior
{
    public override Vector3 Process(double delta)
    {
        float speed = MovementSpeed();
        Vector3 velocity = Entity.Velocity;

        if (Input.IsActionJustPressed("jump") && Entity.IsOnFloor()) velocity.Y += Controller.JumpVelocity;
        
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

    public override bool IsSneaking()
    {
        return Input.IsActionPressed("sneak");
    }

    public  override bool IsSprinting()
    {
        return Input.IsActionPressed("sprint");
    }
}