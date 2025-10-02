using Godot;

namespace RougeLiteGame.entity.behavior;

[GlobalClass] public abstract partial class ReactionBehavior : Behavior
{
    protected IdleBehavior DefaultBehavior { get; private set; }

    public Entity Target { get; private set; }
    
    public void React(Entity target, IdleBehavior defaultBehavior)
    {
        DefaultBehavior = defaultBehavior;
        Target = target;
        
        React();
    }
    
    protected virtual void React() { }
}