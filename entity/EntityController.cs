using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Godot.Collections;
using RougeLiteGame.entity.behavior;
using RougeLiteGame.entity.behavior.idle;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity;

/// <summary>
/// Represents the base class for controlling entities in the game.
/// </summary>
[GlobalClass]
public sealed partial class EntityController : NavigationAgent3D
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(NavigationIdleBehavior));
    
    #region Attributes
    private Entity _entity;

    private Entity Entity
    {
        get => _entity;
        set
        {
            _entity = value;
            SetPhysicsProcess(value != null);
        }
    }

    private MoveState MovementState { get; set; } = MoveState.Stand;
    private Behavior _currentBehaviour;
    private Array<string> _duplicated = [];

    private Behavior CurrentBehaviour
    {
        get => _currentBehaviour;
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        set // Moved to setter because it's the fcking 5'th time I forgot to free the behavior before
        {
            _currentBehaviour?.Uninitialize();
            
            // duplicate behavior per instance to prevent shared routes among copied enemies.
            Behavior newBehavior = CloneBehavior(value);
            _currentBehaviour = newBehavior;
            
            newBehavior.Initialize(this, Entity);
            SetProcessInput(newBehavior is IInputAcceptor);
        }
    }

    /// <summary>
    /// Creates a duplicate of the specified <see cref="Behavior"/> instance to make sure
    /// behavior instances are not shared among copied entities.
    /// </summary>
    /// <param name="behavior">
    /// The <see cref="Behavior"/> to be duplicated. This should be an instance of a behavior
    /// attached to an entity.
    /// </param>
    /// <returns>
    /// A new instance of the provided <see cref="Behavior"/> if it has not been duplicated before,
    /// or the already duplicated <see cref="Behavior"/> instance from the internal cache.
    /// </returns>
    private Behavior CloneBehavior(Behavior behavior)
    {
        if (_duplicated.Contains(behavior.ResourcePath))
        {
            Logger.Trace("Behavior already duplicated: [Type: {}, ResourcePath: {}]. Reusing existing instance for {}.", behavior.GetType().Name, behavior.ResourcePath, Entity.Name);
            return behavior;
        }
        
        Logger.Debug("Duplicating behavior: [Resource: {}, Path: {}] for {}.", behavior.GetType().Name, behavior.ResourcePath, Entity.Name);
        Behavior duplicated = (Behavior)behavior.Duplicate();
        _duplicated.Add(behavior.ResourcePath);
        
        return duplicated;
    }

    #endregion
    
    [ExportGroup("Behavior")]
    [Export] private behavior.idle.IdleBehavior _idleBehavior;
    [ExportSubgroup("Reaction")]

    #region Editor Gravity Settings

    [ExportGroup("Gravity")] 
    [Export] private bool EnableGravity { get; set; } = true;
    [Export] private float GravityMultiplier { get; set; } = 1.14f;
    #endregion Gravity

    #region Editor Movement Settings
    [ExportGroup("Movement")] 
    [Export] private float BaseSpeed { get; set; } = 3f;
    [Export] public float JumpVelocity { get; private set; } = 4.5f;
    [Export] private float SprintAddition { get; set; } = 2.0f;
    [Export] private float SneakPenalty { get; set; } = 2.0f;
    #endregion
    
    #region Debug
    [ExportGroup("Debug")] 
    [Export] private Label _debugLabel;
    #endregion

    public override void _Ready()
    {
        SetPhysicsProcess(IsEntityConnected());

        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 movement = MovementProcess(delta);
        MovementState = ComputeMovementState(movement);

        if (MovementState == MoveState.Fall) movement += ComputeGravity(delta);

        if (_debugLabel != null) _debugLabel.Text = MovementState.ToString();

        // apply movement to the connected entity
        Entity.Velocity = movement;
        Entity.MoveAndSlide();
        base._PhysicsProcess(delta);
    }
    
    public override void _Input(InputEvent @event)
    {
        if (CurrentBehaviour is not IInputAcceptor acceptor) return;
        
        acceptor.AcceptInput(@event);
        base._Input(@event);
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
    /// and only by the entity specified in the editor. Later calls will throw an exception.
    /// Enables physics processing after connection.
    /// </remarks>
    public void Connect(Entity entity)
    {
        if (IsEntityConnected()) throw new InvalidOperationException("The entity has already been connected.");

        Entity = entity;
        SetPhysicsProcess(true);
        
        CurrentBehaviour = _idleBehavior;
    }

    /// <summary>
    /// Sets the idle behavior for this entity controller, optionally overriding the current behavior.
    /// </summary>
    /// <param name="idleBehavior">
    /// The <see cref="behavior.idle.IdleBehavior"/> to set as the idle behavior for the entity.
    /// </param>
    /// <param name="switchBehavior">
    /// A boolean value indicating whether the current behavior should be overridden with the provided idle behavior.
    /// If set to true, the current behavior will be replaced.
    /// </param>
    public void SetIdleBehavior(behavior.idle.IdleBehavior idleBehavior, bool switchBehavior = true)
    {
        _idleBehavior = idleBehavior;
        if (CurrentBehaviour is ReactionBehavior reactionBehavior)
        {
            reactionBehavior.DefaultBehaviorOverride(idleBehavior);
        }
        
        if (!switchBehavior) return;
        
        CurrentBehaviour = idleBehavior;
    }

    /// <summary>
    /// Sets the reaction behavior for the entity and triggers the corresponding reaction process.
    /// </summary>
    /// <param name="behavior">
    /// The <see cref="ReactionBehavior"/> to be set as the current behavior for the entity.
    /// </param>
    /// <param name="target">
    /// The target <see cref="Entity"/> that the reaction behavior will respond to.
    /// </param>
    public void SetReactionBehavior(ReactionBehavior behavior, Entity target)
    {
        ReactionBehavior reactionBehavior = (ReactionBehavior)CloneBehavior(behavior);
        reactionBehavior.React(target, _idleBehavior);
        CurrentBehaviour = behavior;
    }
    
    /// <summary>
    /// Returns whether an entity is connected to this instance.
    /// </summary>
    /// <returns>
    /// <c>true</c> if an entity has been connected to this instance; otherwise, <c>false</c>.
    /// </returns>
    private bool IsEntityConnected()
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
    private MoveState ComputeMovementState(Vector3 movement)
    {
        if (!Entity.IsOnFloor() && EnableGravity) return MoveState.Fall;
        if (movement == Vector3.Zero) return MoveState.Stand;
        if (CurrentBehaviour.IsSneaking()) return MoveState.Sneak;

        return CurrentBehaviour.IsSprinting() ? MoveState.Sprint : MoveState.Walk;
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
    private Vector3 MovementProcess(double delta)
    {   
        // Do not query when the map has never synchronized and is empty.
        // Our Entity will just freeze when no navigation mesh is existent.
        
        bool navigationMeshExists = NavigationServer3D.MapGetIterationId(GetNavigationMap()) != 0;
        return !navigationMeshExists ? Vector3.Zero : CurrentBehaviour.Process(delta);
    }

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
    public float MovementSpeed()
    {
        if (CurrentBehaviour.IsSneaking()) return BaseSpeed / SneakPenalty;
        if (!CurrentBehaviour.IsSprinting()) return BaseSpeed;

        return BaseSpeed * SprintAddition;
    }
    
    /// <summary>
    /// Represents the different movement states that an entity can have within the game.
    /// </summary>
    /// <remarks>
    /// The movement state determines how an entity's motion and behavior are processed.
    /// It includes states for stationary, basic walking, sprinting, sneaking, and falling.
    /// </remarks>
    private enum MoveState
    {
        Stand,
        Walk,
        Sprint,
        Fall,
        Sneak
    }
}