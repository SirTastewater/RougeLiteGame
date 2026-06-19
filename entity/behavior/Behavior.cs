using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
// ReSharper disable MemberCanBePrivate.Global

namespace RougeLiteGame.entity.behavior;

[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
[GlobalClass] public abstract partial class Behavior : Resource
{
    // I am not going to write documentation just yet. Because I have enough!
    
    protected EntityController Controller { get; private set; }
    protected Entity Entity { get; private set; }
    
    protected Vector3 Velocity;

    public void Initialize(EntityController controller, Entity entity)
    {
        if (Controller != null || Entity != null)
        {
            throw new InvalidOperationException("The behavior has already been initialized.");
        }
        
        Controller = controller;
        Entity = entity;
        
        Init();
    }
    
    protected Node GetNode(NodePath path)
    {
        return Controller.GetNode<Node>(path);
    }

    protected T GetNode<T>(NodePath path) where T: Node
    {
        return Controller.GetNode<T>(path);
    }
    
    protected virtual void Init() { }
    
    public void Uninitialize()
    {
        if (Controller == null && Entity == null)
        {
            throw new InvalidOperationException("The behavior is currently uninitialized.");
        }
        
        Controller = null;
        Entity = null;
    }

    public Vector3 InitiateProcess(double delta)
    {
        Velocity = Entity.Velocity;
        Process(delta);
        return Velocity;
    }
    
    protected abstract void Process(double delta);

    protected Vector3 FollowControllerTarget()
    {
        return Entity.GlobalPosition.DirectionTo(Controller.GetNextPathPosition()) * Controller.MovementSpeed();
    }
    
    protected void Jump()
    {
        Velocity += Controller.ComputeJumpVelocity();
        Controller.EmitSignal(EntityController.SignalName.Jump, Entity);
    }

    protected float MovementSpeed()
    {
        return Controller.MovementSpeed();
    }
    
    protected Vector3 GetTargetDistance()
    {
        return Controller.GetTargetPosition() - Entity.GlobalPosition;
    }

    protected Vector3 GetTargetPosition()
    {
        return Controller.GetTargetPosition();
    }
    
    protected void SetTargetPosition(Vector3 position)
    {
        Controller.SetTargetPosition(position);
    }

    public virtual bool IsSprinting()
    {
        return false;
    }

    public virtual bool IsSneaking()
    {
        return false;
    }
}