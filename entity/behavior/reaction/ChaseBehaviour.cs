using Godot;

namespace RougeLiteGame.entity.behavior.reaction;

[GlobalClass] public partial class ChaseBehaviour(Entity target) : ReactionBehavior
{
    public override Vector3 Process(double delta)
    {
        SetTargetPosition(target.GlobalPosition);
        Entity.LookAt(target.GlobalPosition);
        return FollowControllerTarget();
    }
}