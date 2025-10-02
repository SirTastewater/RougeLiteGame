using Godot;

namespace RougeLiteGame.entity.behavior;

[GlobalClass] public abstract partial class ReactionBehavior : Behavior
{
    protected IdleBehavior DefaultBehavior { get; private set; }

    protected Entity Target { get; private set; }
    
    public void DefaultBehaviorOverride(IdleBehavior defaultBehavior)
    {
        DefaultBehavior = defaultBehavior;
    }
    
    public void React(Entity target, IdleBehavior defaultBehavior)
    {
        DefaultBehavior = defaultBehavior;
        Target = target;
    }
}