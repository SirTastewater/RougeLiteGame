using Godot;
using Godot.Collections;
using RougeLiteGame.entity.limbs;
using RougeLiteGame.entity.limbs.instance;

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
    private BodyStructure _bodyStructure;
    
    [ExportGroup("Limbs")]
    [Export] private TorsoInstance _torso;
    [Export] private Array<HeadInstance> _headLimbs = [];
    [Export] private Array<ArmInstance> _armLimbs = [];
    [Export] private Array<LegInstance> _legLimbs = [];
    
    public override void _Ready()
    {
        base._Ready();

        _entityController?.Connect(this);
        _bodyStructure = new BodyStructure(_torso, _headLimbs, _armLimbs, _legLimbs);
    }

    public BodyStructure GetLimbs()
    {
        return _bodyStructure;
    }
}