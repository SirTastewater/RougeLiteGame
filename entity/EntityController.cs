using System;
using Godot;

namespace RougeLiteGame.entity;

public abstract partial class EntityController : NavigationAgent3D
{
    protected enum MoveState
    {
        Stand,
        Walk,
        Sprint,
        Fall,
        Sneak
    }

    #region Attributes
    protected Entity Entity { get; private set; }
    protected MoveState MovementState { get; private set; } = MoveState.Stand;

    #endregion
    #region Editor Gravity Settings
    [ExportGroup("Gravity")] 
    [Export] protected bool EnableGravity { get; set; } = true;

    [Export] private float GravityMultiplier { get; set; } = 1.14f;
    #endregion Gravity
    #region Editor Movement Settings
    [ExportGroup("Movement")] 
    [Export] protected float BaseSpeed { get; private set; } = 3f;

    [Export] protected float JumpVelocity { get; private set; } = 4.5f;

    [Export] protected float SprintAddition { get; private set; } = 2.0f;
    [Export] protected float SneakPenalty { get; private set; } = 2.0f;
    #endregion
    
    #region Debug
    [ExportGroup("Debug")] 
    [Export] private Label _debugLabel;
    #endregion
    
    public override void _Ready()
    {
        // idk how godot works, so imma just do that
        SetPhysicsProcess(IsEntityConnected());
        SetProcessInput(false);
        
        base._Ready();
    }
    
    /**
     * This method should only be called by the entity which is set in editor
     * If called more than once, an exception will be thrown.
     */
    public void Connect(Entity entity)
    {
        if (IsEntityConnected())
        {
            throw new InvalidOperationException("The entity has already been connected.");
        }
        
        Entity = entity;
        SetPhysicsProcess(true);
    }

    private bool IsEntityConnected()
    {
        return Entity != null;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Entity == null)
        {
            SetPhysicsProcess(false);
            return;
        }
        
        Vector3 movement = MovementProcess(delta);
        MovementState = ComputeMovementState(movement);

        if (MovementState == MoveState.Fall)
        {
            movement += ComputeGravity(delta);
        }

        if (_debugLabel != null)
        {
            _debugLabel.Text = MovementState.ToString();
        }

        // apply movement to the connected entity
        Entity.Velocity = movement;
        Entity.MoveAndSlide();
        base._PhysicsProcess(delta);
    }

    private MoveState ComputeMovementState(Vector3 movement)
    {
        if (!Entity.IsOnFloor() && EnableGravity)
        {
            return MoveState.Fall;
        }

        if (movement == Vector3.Zero)
        {
            return MoveState.Stand;
        }

        if (IsSneaking())
        {
            return MoveState.Sneak;
        }

        if (IsSprinting())
        {
            return MoveState.Sprint;
        }

        return MoveState.Walk;
    }

    protected abstract Vector3 MovementProcess(double delta);

    private Vector3 ComputeGravity(double delta)
    {
        return GravityMultiplier * Entity.GetGravity() * (float)delta;
    }

    protected float MovementSpeed()
    {
        if (IsSneaking())
        {
            return BaseSpeed / SneakPenalty;   
        }
        
        if (!IsSprinting()) return BaseSpeed;

        
        return BaseSpeed * SprintAddition;
    }

    protected virtual bool IsSprinting()
    {
        return false;
    }
    
    protected virtual bool IsSneaking()
    {
        return false;
    }
}