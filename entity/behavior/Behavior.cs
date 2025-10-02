using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
// ReSharper disable MemberCanBePrivate.Global

namespace RougeLiteGame.entity.behavior;

[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
[GlobalClass] public abstract partial class Behavior : Resource
{
    protected EntityController Controller { get; private set; }
    protected Entity Entity { get; private set; }
    
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
    
    public abstract Vector3 Process(double delta);

    protected Vector3 FollowControllerTarget()
    {
        Vector3 nextPathPosition = Controller.GetNextPathPosition();
        Vector3 desiredMovement = Entity.GlobalPosition.DirectionTo(nextPathPosition) * Controller.MovementSpeed();

        if (!Controller.AvoidanceEnabled) return desiredMovement;
        
        // Godot's setter uses black magic (or smt) to space out enemies following the same position
        Controller.Velocity = desiredMovement;
        return desiredMovement;
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