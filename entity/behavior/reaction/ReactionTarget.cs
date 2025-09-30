using Godot;

namespace RougeLiteGame.entity.behavior.reaction;

[GlobalClass] public partial class ReactionTarget : Resource
{
    [Export(PropertyHint.NodePathValidTypes, "Entity")] public NodePath Entity { get; private set; }
    [Export] public ReactionBehavior Behavior { get; private set; }

    public ReactionTarget() { }

    public ReactionTarget(NodePath entity, ReactionBehavior behavior)
    {
        Entity = entity;
        Behavior = behavior;
    }
}