using Godot;

namespace RougeLiteGame.entity.behavior.reaction;

[GlobalClass] public partial class ChaseBehaviour(Entity target) : ReactionBehavior
{
    protected override void Process(double delta)
    {
        SetTargetPosition(target.GlobalPosition);
        Entity.LookAt(target.GlobalPosition);
        Velocity = FollowControllerTarget();
    }
}