using Godot;

namespace RougeLiteGame.entity.behavior.reaction;

[GlobalClass] public partial class ChaseBehaviour : ReactionBehavior
{
    public override Vector3 Process(double delta)
    {
        SetTargetPosition(Target.GlobalPosition);
        Entity.LookAt(Target.GlobalPosition);
        
        return FollowControllerTarget();
    }

    public override bool ReactionDone()
    {
        return Controller.IsNavigationFinished();
    }
}