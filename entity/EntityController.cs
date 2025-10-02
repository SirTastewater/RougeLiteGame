using System;
using Godot;
using RougeLiteGame.entity.camera;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity;

/// <summary>
/// Represents the base class for controlling entities in the game.
/// </summary>
public abstract partial class EntityController : NavigationAgent3D
{
    #region Attributes

    private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(EntityController));

    protected Entity Entity { get; private set; }
    protected MoveState MovementState { get; private set; } = MoveState.Stand;

    #endregion

    #region Editor Gravity Settings

    [ExportGroup("Gravity")] [Export] protected bool EnableGravity { get; set; } = true;

    [Export] private float GravityMultiplier { get; set; } = 1.14f;

    #endregion Gravity

    #region Editor Movement Settings

    [ExportGroup("Movement")] [Export] protected float BaseSpeed { get; private set; } = 3f;

    [Export] protected float JumpVelocity { get; private set; } = 4.5f;

    [Export] protected float SprintAddition { get; private set; } = 2.0f;
    [Export] protected float SneakPenalty { get; private set; } = 2.0f;

    #endregion
    
    #region Debug

    [ExportGroup("Debug")] [Export] private Label _debugLabel;

    #endregion

    public override void _Ready()
    {
        // idk how godot works, so imma just do that
        SetPhysicsProcess(IsEntityConnected());
        SetProcessInput(false);
        Logger.Log("EntityController ready.");

        base._Ready();
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

        if (MovementState == MoveState.Fall) movement += ComputeGravity(delta);

        if (_debugLabel != null) _debugLabel.Text = MovementState.ToString();

        // apply movement to the connected entity
        Entity.Velocity = movement;
        Entity.MoveAndSlide();
        base._PhysicsProcess(delta);
    }

    /// <summary>
    /// Connects the specified <see cref="Entity"/> to this instance.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="Entity"/> to connect. This should be the entity set via the editor.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once or if an entity is already connected.
    /// </exception>
    /// <remarks>
    /// This method should only be called once during the lifecycle of the object
    /// and only by the entity specified in the editor. Subsequent calls will throw an exception.
    /// Enables physics processing after connection.
    /// </remarks>
    public void Connect(Entity entity)
    {
        if (IsEntityConnected()) throw new InvalidOperationException("The entity has already been connected.");

        Entity = entity;
        SetPhysicsProcess(true);
        Logger.Log("Entity {} has been connected.", Entity.Name);
        
        EntityReady();
    }

    protected virtual void EntityReady() {}
    
    /// <summary>
    /// Returns whether an entity is connected to this instance.
    /// </summary>
    /// <returns>
    /// <c>true</c> if a entity has been connected to this instance; otherwise, <c>false</c>.
    /// </returns>
    protected bool IsEntityConnected()
    {
        return Entity != null;
    }

    /// <summary>
    /// Computes and returns the current movement state of the entity based on input movement,
    /// gravity status, and movement mode.
    /// </summary>
    /// <param name="movement">
    /// A <see cref="Vector3"/> representing the movement direction and magnitude of the entity.
    /// </param>
    /// <returns>
    /// A <see cref="MoveState"/> value representing the entity's current movement state.
    /// </returns>
    /// <remarks>
    /// The method determines the state in the following order:
    /// <list type="number">
    /// <item><description>If the entity is not on the floor and gravity is enabled, returns <c>Fall</c>.</description></item>
    /// <item><description>If there is no movement input, returns <c>Stand</c>.</description></item>
    /// <item><description>If the entity is sneaking, returns <c>Sneak</c>.</description></item>
    /// <item><description>If the entity is sprinting, returns <c>Sprint</c>, otherwise returns <c>Walk</c>.</description></item>
    /// </list>
    ///
    /// It is important to note that a falling entity is not able to also sneak, sprint or walk.
    ///
    /// </remarks>
    /// TODO: Discuss the above order with the whole team!!! (Jan and me)
    private MoveState ComputeMovementState(Vector3 movement)
    {
        if (!Entity.IsOnFloor() && EnableGravity) return MoveState.Fall;
        if (movement == Vector3.Zero) return MoveState.Stand;
        if (IsSneaking()) return MoveState.Sneak;

        return IsSprinting() ? MoveState.Sprint : MoveState.Walk;
    }

    /// <summary>
    /// Processes input and other movement-related factors to compute and return the resulting velocity vector.
    /// </summary>
    /// <param name="delta">
    /// The frame delta time, typically used to scale movement by time to ensure consistent behavior across frame rates.
    /// </param>
    /// <returns>
    /// A <see cref="Vector3"/> representing the movement or velocity vector computed for this frame.
    /// </returns>
    /// <remarks>
    /// This method is intended to be implemented by derived classes to define custom movement behavior.
    /// </remarks>
    protected abstract Vector3 MovementProcess(double delta);

    /// <summary>
    /// Computes the gravitational force to be applied to the entity over the specified time delta.
    /// </summary>
    /// <param name="delta">
    /// The elapsed time in seconds since the last physics update. This value is used to calculate the incremental force due to gravity.
    /// </param>
    /// <returns>
    /// A <see cref="Vector3"/> representing the gravitational force to be applied to the entity, adjusted by the gravity multiplier.
    /// </returns>
    private Vector3 ComputeGravity(double delta)
    {
        return GravityMultiplier * Entity.GetGravity() * (float) delta;
    }


    /// <summary>
    /// Calculates and returns the current movement speed of the entity.
    /// </summary>
    /// <returns>
    /// The movement speed based on the current state (base speed, adjusted for sneaking or sprinting).
    /// </returns>
    /// <remarks>
    /// This method determines the effective movement speed by considering whether the entity
    /// is sneaking, sprinting, or in a normal state. Sneaking reduces the speed relative to the
    /// base speed by the sneaking penalty factor, while sprinting increases it by the sprint addition factor.
    /// </remarks>
    protected float MovementSpeed()
    {
        if (IsSneaking()) return BaseSpeed / SneakPenalty;

        if (!IsSprinting()) return BaseSpeed;


        return BaseSpeed * SprintAddition;
    }

    /// <summary>
    /// Determines whether the entity is currently in a sprinting state.
    /// </summary>
    /// <returns>
    /// True if the entity is sprinting; otherwise, false.
    /// </returns>
    /// <remarks>
    /// This method can be overridden in derived classes to implement specific logic
    /// for determining the sprinting state.
    /// </remarks>
    protected virtual bool IsSprinting()
    {
        return false;
    }

    /// <summary>
    /// Checks if the entity is in a sneaking state based on specific input or conditions.
    /// </summary>
    /// <returns>
    /// True if the entity is sneaking, otherwise false. Sneaking generally applies movement penalties
    /// or reduced visibility impact, depending on game logic.
    /// </returns>
    /// <remarks>
    /// The sneaking state may affect movement speed or other gameplay mechanics. Override this method
    /// in derived classes to implement specific sneaking behavior.
    /// </remarks>
    protected virtual bool IsSneaking()
    {
        return false;
    }

    /// <summary>
    /// Represents the different movement states that an entity can have within the game.
    /// </summary>
    /// <remarks>
    /// The movement state determines how an entity's motion and behavior are processed.
    /// It includes states for stationary, basic walking, sprinting, sneaking, and falling.
    /// </remarks>
    protected enum MoveState
    {
        Stand,
        Walk,
        Sprint,
        Fall,
        Sneak
    }

}