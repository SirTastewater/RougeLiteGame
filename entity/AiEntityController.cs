using Godot;
using Godot.Collections;
using RougeLiteGame.entity.behavior;
using ReactionTarget = RougeLiteGame.entity.behavior.reaction.ReactionTarget;

namespace RougeLiteGame.entity;

[GlobalClass]
public partial class AiEntityController : EntityController
{
    [Export] private Area3D _detectionArea;

    [Export] private Array<ReactionTarget> _behaviours;
    [Export] private IdleBehavior _idleBehavior;

    private AiBehavior _currentBehaviour;

    private AiBehavior CurrentBehaviour
    {
        get => _currentBehaviour;
        set // Moved to setter because it's the fcking 5'th time I forgot to free the behavior before
        {
            _currentBehaviour?.Uninitialize();
            _currentBehaviour = value;
            value.Initialize(this, Entity);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        if(_detectionArea == null) return;
        _detectionArea.BodyEntered += BodyEntered;
    }

    protected override void EntityReady()
    {
        base.EntityReady();
        CurrentBehaviour = _idleBehavior;
    }

    private void BodyEntered(Node3D body)
    {
        if (body is not Entity target)
        {
            return;
        }

        foreach (ReactionTarget behaviour in _behaviours)
        {
            if (GetNode<Entity>(behaviour.Entity) != target) continue;
            
            CurrentBehaviour = behaviour.Behavior;
            behaviour.Behavior.React(target, _idleBehavior);
        }
    }

    protected override Vector3 MovementProcess(double delta)
    {
        if(CurrentBehaviour == null) return Vector3.Zero;
        
        // Do not query when the map has never synchronized and is empty.
        // Our Entity will just freeze when no navigation mesh is existent.
        if (NavigationServer3D.MapGetIterationId(GetNavigationMap()) == 0) return Vector3.Zero;
        
        if (CurrentBehaviour is not ReactionBehavior reactBehavior || !reactBehavior.Release())
            return CurrentBehaviour.Process(delta);

        CurrentBehaviour = _idleBehavior;

        return CurrentBehaviour.Process(delta);
    }

    protected override bool IsSprinting()
    {
        return CurrentBehaviour.IsSprinting();
    }

    protected override bool IsSneaking()
    {
        return CurrentBehaviour.IsSneaking();
    }
}