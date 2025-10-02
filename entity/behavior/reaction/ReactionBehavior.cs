using Godot;

namespace RougeLiteGame.entity.behavior;

[GlobalClass] public abstract partial class ReactionBehavior : Behavior
{
    protected idle.IdleBehavior DefaultBehavior { get; private set; }

    protected Entity Target { get; private set; }
    
    public void DefaultBehaviorOverride(idle.IdleBehavior defaultBehavior)
    {
        DefaultBehavior = defaultBehavior;
    }
    
    public void React(Entity target, idle.IdleBehavior defaultBehavior)
    {
        DefaultBehavior = defaultBehavior;
        Target = target;
    }
}