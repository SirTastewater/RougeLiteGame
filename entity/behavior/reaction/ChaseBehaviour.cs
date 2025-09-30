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

    public override bool IsSprinting()
    {
        return false;
    }

    public override bool Release()
    {
        return Controller.IsNavigationFinished();
    }
}