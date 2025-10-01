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
    
    public Node GetNode(NodePath path)
    {
        return Controller.GetNode<Node>(path);
    }
    
    public T GetNode<T>(NodePath path) where T: Node
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

    protected bool IsFacingTarget(float tolerance = 0.01f)
    {
        return IsFacing(GetTargetPosition(), tolerance);
    }
    
    protected bool IsFacing(Vector3 location, float tolerance = 0.01f)
    {
        return AngleTo(location) < tolerance;
    }

    protected float AngleTo(Vector3 location)
    {
        Vector3 target = location - Entity.GlobalPosition;
        target.Y = 0;
        target.Normalized();
                
        Vector3 facing = -Entity.GlobalTransform.Basis.Z;
        facing.Y = 0;
        facing = facing.Normalized();

        return facing.AngleTo(target);
    }

    protected float RotateTowards(double delta, float rotationStep = 2.0f)
    {
        return RotateTowards(GetTargetPosition(), delta, rotationStep);
    }
    
    protected float RotateTowards(Vector3 location, double delta, float rotationStep = 2.0f)
    {
        Vector3 target = GetTargetPosition() - Entity.GlobalPosition;
        target.Y = 0;
        target.Normalized();
                
        Vector3 facing = -Entity.GlobalTransform.Basis.Z;
        facing.Y = 0;
        facing = facing.Normalized();

        float angle = facing.AngleTo(target);
        float cross = facing.Cross(target).Y;
        float turnDirection = float.Sign(cross);
        float rotationAmount = Mathf.Min(angle, rotationStep * (float) delta);
            
        Entity.Rotation = new Vector3(Entity.Rotation.X, Entity.Rotation.Y + turnDirection * rotationAmount, Entity.Rotation.Z);
        return angle;
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