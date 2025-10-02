using System;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity.behavior;

[GlobalClass] public partial class NavigationIdleBehavior : IdleBehavior
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(NavigationIdleBehavior));
    
    private Vector3 _targetPosition;

    protected override void Init()
    {
        SetTargetPosition(GetNextPosition());
    }

    public override Vector3 Process(double delta)
    {
        if (!Entity.IsOnFloor()) return Entity.Velocity;

        if (GetTargetPosition() != Vector3.Zero && !Controller.IsNavigationFinished())
        {
            if (!IsFacingTarget()) RotateTowards(delta);

            return FollowControllerTarget();
        }
        
        SetTargetPosition(GetNextPosition());
        return FollowControllerTarget();
    }

    /// <summary>
    /// Determines whether the entity is facing its current target position within a specified angular tolerance.
    /// See <see cref="IsFacing(Vector3, float)"/> for checking against an arbitrary location.
    /// </summary>
    /// <param name="tolerance">The angular tolerance in radians for determining if the entity is facing the target position.</param>
    /// <returns>
    /// A boolean indicating whether the entity is facing the target position within the specified tolerance.
    /// </returns>
    protected bool IsFacingTarget(float tolerance = 0.01f)
    {
        return IsFacing(GetTargetPosition(), tolerance);
    }

    /// <summary>
    /// Determines if the entity is facing a specified target location on the XZ plane,
    /// within a given angular tolerance.
    /// </summary>
    /// <param name="location">The target position in world coordinates.</param>
    /// <param name="tolerance">The angular tolerance in radians for determining if the entity is facing the target.</param>
    /// <returns>
    /// A boolean indicating whether the entity is facing the specified target location
    /// within the provided tolerance.
    /// </returns>
    protected bool IsFacing(Vector3 location, float tolerance = 0.01f)
    {
        return AngleTo(location) < tolerance;
    }

    /// <summary>
    /// Calculates the angle in radians between the entity's current facing direction
    /// and a specified target location on the XZ plane.
    /// </summary>
    /// <param name="location">The target position in world coordinates.</param>
    /// <returns>
    /// The angle in radians between the entity's facing direction and the direction
    /// to the specified target position.
    /// </returns>
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

    /// <summary>
    /// Rotates the entity towards the specified target position, adjusting its orientation incrementally.
    /// See <see cref="RotateTowards(Vector3, double, float)"/> for more control over the rotation.
    /// </summary>
    /// <param name="delta">The frame time in seconds, used to scale the rotation step.</param>
    /// <param name="rotationStep">The maximum allowable rotation step in radians per second.</param>
    /// <returns>
    /// The remaining angle in radians between the entity's current orientation and the target direction after applying the rotation.
    /// </returns>
    protected float RotateTowards(double delta, float rotationStep = 2.0f)
    {
        return RotateTowards(GetTargetPosition(), delta, rotationStep);
    }

    /// <summary>
    /// Rotates the entity towards a specified target location on the XZ plane,
    /// adjusting its rotation incrementally based on the provided delta time and rotation step.
    /// </summary>
    /// <param name="location">The target position to rotate towards in world coordinates.</param>
    /// <param name="delta">The delta time to scale the rotation adjustment.</param>
    /// <param name="rotationStep">The maximum rotation step to apply per second.</param>
    /// <returns>
    /// The angle in radians between the entity's current-facing direction and the target direction after rotation.
    /// </returns>
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

    /// <summary>
    /// Determines the next target position for the entity to navigate to.
    /// </summary>
    /// <returns>
    /// A 3D vector representing the next position in the world coordinates that
    /// the entity should move toward.
    /// </returns>
    /// <remarks>
    /// The default behavior is to generate a random point to walk to.
    /// It can, however, be overriden. 
    /// </remarks>
    protected virtual Vector3 GetNextPosition()
    {
        Logger.Trace("Generating new random position for basic navigation.");
        
        Vector3 randomPosition;
        do
        {
            randomPosition = new Vector3(GD.RandRange(-10, 10), 1.4f, GD.RandRange(-10, 10));
        } while(randomPosition.DistanceTo(Entity.GlobalPosition) < 4);
        
        return randomPosition;
    }
}