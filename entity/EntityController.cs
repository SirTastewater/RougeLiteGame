using System;
using Godot;

namespace RougeLiteGame.entity;

public abstract partial class EntityController : NavigationAgent3D
{
    protected Entity Entity { get; private set; }

    protected MoveState MovementState { get; private set; } = MoveState.Stand;

    [ExportGroup("Essential")] 
    [Export] protected bool EnableGravity { get; set; } = true;
    [Export] private float GravityMultiplier { get; set; } = 1.14f;
    
    
    [ExportGroup("Movement")] 
    [Export] protected float BaseSpeed { get; private set; } = 3f;
    [Export] protected float JumpVelocity { get; private set; } = 4.5f;
    [Export] protected float SprintAddition { get; private set; } = 2.0f;

    public void Connect(Entity entity)
    {
        if (IsEntityConnected())
        {
            throw new InvalidOperationException("The entity has already been connected.");
        }
        
        Entity = entity;
    }

    private bool IsEntityConnected()
    {
        return Entity != null;
    }

    public override void _PhysicsProcess(double delta)
    {
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
        else
        {
            MovementState = MoveState.Walk;
        }

        Entity.Velocity = movement;
        Entity.MoveAndSlide();
        base._PhysicsProcess(delta);
    }

    protected abstract Vector3 MovementProcess(double delta);
    
    private Vector3 ComputeGravity(double delta)
    {
        return GravityMultiplier * Entity.GetGravity() * (float)delta;
    }

    protected virtual float MovementSpeed()
    {
        return BaseMovementSpeed();
    }

    private float BaseMovementSpeed()
    {
        return 1;
    }

    protected enum MoveState
    {
        Stand,
        Walk,
        Sprint,
        Fall,
        Sneak
    }
}