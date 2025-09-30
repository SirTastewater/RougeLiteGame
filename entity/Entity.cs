using Godot;
using RougeLiteGame.entity.behavior;
using RougeLiteGame.entity.camera;
using ReactionTarget = RougeLiteGame.entity.behavior.reaction.ReactionTarget;

namespace RougeLiteGame.entity;

/// <summary>
/// Represents a base class for all game entities, inheriting from <see cref="Godot.CharacterBody3D"/>.
/// </summary>
/// <remarks>
/// This abstract class serves as a common foundation for various entities within the game, such as players or NPCs.
/// </remarks>
// TODO: This class is currently just a placeholder and will need a lot of work still. Therefore this documentation is incomplete.
[GlobalClass]
public abstract partial class Entity : CharacterBody3D
{
    [Export] private EntityController _entityController;

    public ReactionTarget ToReactionTarget(ReactionBehavior behavior)
    {
        return new ReactionTarget(GetPath(), behavior);
    }

    public override void _Ready()
    {
        base._Ready();

        _entityController?.Connect(this);
    }
}