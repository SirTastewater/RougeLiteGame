using Godot;

namespace RougeLiteGame.entity.behavior;

[GlobalClass] public abstract partial class ReactionBehavior : AiBehavior
{
    
    protected IdleBehavior DefaultBehavior { get; private set; }
    
    protected Entity Target { get; private set; }
    
    public void React(Entity target, IdleBehavior defaultBehavior)
    {
        DefaultBehavior = defaultBehavior;
        Target = target;
        
        React();
    }

    protected virtual void React() { }

    public abstract bool Release();
}