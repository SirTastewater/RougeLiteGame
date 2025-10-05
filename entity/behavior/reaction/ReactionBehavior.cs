using Godot;
using Godot.Collections;
using RougeLiteGame.entity.behavior.idle;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity.behavior.reaction;

[GlobalClass] public abstract partial class ReactionBehavior : Behavior
{
    protected IdleBehavior DefaultBehavior { get; private set; }
    
    public void SetDefaultBehavior(IdleBehavior defaultBehavior)
    {
        DefaultBehavior = defaultBehavior;
    }
}