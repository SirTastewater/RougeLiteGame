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

        if (!Entity.IsOnFloor() && EnableGravity)
        {
            movement += ComputeGravity(delta);
            MovementState = MoveState.Fall;
        }
        else if(movement == Vector3.Zero)
        {
            MovementState = MoveState.Stand;
        }
        else if (IsSprinting())
        {
            MovementState = MoveState.Sprint;    
        }
        else 
        {
            MovementState = MoveState.Walk;
        }

        // apply movement to the connected entity
        Entity.Velocity = movement;
        Entity.MoveAndSlide();
        base._PhysicsProcess(delta);
    }

    protected abstract Vector3 MovementProcess(double delta);

    private Vector3 ComputeGravity(double delta)
    {
        return GravityMultiplier * Entity.GetGravity() * (float)delta;
    }

    protected float MovementSpeed()
    {
        if (!IsSprinting()) return BaseSpeed;
        
        return BaseSpeed * SprintAddition;
    }

    protected virtual bool IsSprinting()
    {
        return false;
    }
}