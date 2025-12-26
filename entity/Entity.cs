using System.Collections.Generic;
using System.Linq;
using Godot;
using RougeLiteGame.entity.limbs;
using RougeLiteGame.logger;

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
    private static readonly ILogger Logger = LoggerFactory.GetLogger<Entity>();
    [Export] private EntityController _entityController;

    public float SpeedGain => _limbs.Sum(limb => limb.Speed);
    public float StrengthGain => _limbs.Sum(limb => limb.Strength);
    public float LifeGain() => _limbs.Sum(limb => limb.Life);
    
    private List<Limb> _limbs = [];
    
    public override void _Ready()
    {
        base._Ready();

        EvaluateLimbs(); // can be above the line below because they don't need an entity controller ...yet
        _entityController?.Connect(this);
    }

    private void EvaluateLimbs()
    {
        Logger.Info("{}: Reevaluating limbs...", GetName());
        if (_limbs.Count > 0)
        {
            foreach (Limb limb in _limbs)
            {
                Logger.Trace("{}: Uninitialized limb {}.", GetName(), limb.Name);
                limb.Uninitialize();
            }
            Logger.Info("{}: Uninitialized {} limbs.", GetName(), _limbs.Count);
            
            _limbs.Clear();
        }
        
        foreach (Node child in GetChildren())
        {
            if (child is not Limb { Visible: true } limb)
            {
                Logger.Trace("{}: Skip adding child {}, as this was identified to be no limb.", GetName(), child.GetName());
                continue;
            }
            Logger.Trace("{}: Adding limb {}.", GetName(), limb.Name);
            
            limb.Initialize(this);
            _limbs.Add(limb);
        }
        Logger.Info("{}: Finish reevaluating. Found {} limbs.", GetName(), _limbs.Count);
    }
}