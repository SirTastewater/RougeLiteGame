using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Godot.Collections;
using RougeLiteGame.entity.behavior;
using RougeLiteGame.entity.behavior.idle;
using RougeLiteGame.entity.behavior.reaction;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity;

/// <summary>
/// Represents the base class for controlling entities in the game.
/// </summary>
[GlobalClass]
public sealed partial class EntityController : NavigationAgent3D
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(EntityController));
    
    #region Attributes
    private Entity _entity;
    private MoveState MovementState { get; set; } = MoveState.Stand;
    private Behavior _currentBehaviour;
    private Array<string> _duplicated = [];

    private Entity Entity { get => _entity; set { _entity = value; SetPhysicsProcess(value != null); } }
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

    private float _currentStamina;
    #endregion
    
    [ExportGroup("Behavior")]
    [Export] private IdleBehavior _idleBehavior;
    [ExportSubgroup("Reaction")]

    #region Editor Gravity Settings

    [ExportGroup("Gravity")] 
    [Export] private bool EnableGravity { get; set; } = true;
    [Export] private float GravityMultiplier { get; set; } = 1.14f;
    #endregion Gravity

    #region Editor Movement Settings
    [ExportGroup("Movement")] 
    [Export] private float BaseSpeed { get; set; } = 3f;
    [Export] public float JumpHeight { get; private set; } = 1f;
    [Export] private float SneakPenalty { get; set; } = 2.0f;
    [ExportSubgroup("Sprint")]
    // I am preparing something big
    [Export] private float MaximumSpeedAddition { get; set; } = 2.0f;
    [Export] private float Stamina { get; set; } = 2.0f;
    [Export] private Curve SprintCurve { get; set; }
    #endregion
    
    public override void _Ready()
    {
        SetPhysicsProcess(IsEntityConnected());
        base._Ready();
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
        VelocityComputed += ApplyMovement;
        _currentStamina = Stamina;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Do not query when the map has never synchronized and is empty.
        // Our Entity will just freeze when no navigation mesh is existent.
        bool navigationMeshExists = NavigationServer3D.MapGetIterationId(GetNavigationMap()) != 0;
        Vector3 movement = !navigationMeshExists ? Vector3.Zero : CurrentBehaviour.Process(delta);
        MovementState = ComputeMovementState(movement);
        
        if (MovementState == MoveState.Fall && EnableGravity) movement += ComputeGravity(delta);
        if (AvoidanceEnabled)
        {
            // Godot uses black magic to compute a vector spacing out agents.
            // When calling the setter of the NavigationsAgent velocity, it will calculate a new velocity spacing out from other
            // agents and send the result via the VelocityComputed signal.
            // more information can be found here:
            // https://docs.godotengine.org/en/4.0/tutorials/navigation/navigation_using_agent_avoidance.html
            Velocity = movement;
            return;
        }
        
        Entity.Velocity = movement;
        ApplyMovement(movement);
    }

    public override void _Input(InputEvent @event)
    {
        if (CurrentBehaviour is not IInputAcceptor acceptor) return;
        
        acceptor.AcceptInput(@event);
        base._Input(@event);
    }

    /// <summary>
    /// Applies the specified movement vector to the connected entity and triggers movement calculations.
    /// Adjusts the entity's velocity and calls its movement logic.
    /// </summary>
    /// <param name="movement">
    /// A <see cref="Vector3"/> representing the movement vector to apply to the entity.
    /// This vector typically considers navigation, user input, and environmental factors such as gravity.
    /// </param>
    private void ApplyMovement(Vector3 movement)
    {
        Entity.Velocity = movement;
        Entity.MoveAndSlide();
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
    public void SetIdleBehavior(IdleBehavior idleBehavior, bool switchBehavior = true)
    {
        _idleBehavior = idleBehavior;
        if (CurrentBehaviour is ReactionBehavior reactionBehavior)
        {
            reactionBehavior.SetDefaultBehavior(idleBehavior);
        }
        
        if (!switchBehavior) return;
        SetBehavior(idleBehavior);
    }

    /// <summary>
    /// Switches the current behavior of the entity to the specified <see cref="Behavior"/>.
    /// </summary>
    /// <param name="behavior">
    /// The <see cref="Behavior"/> to switch to. This should inherit from the <see cref="Behavior"/>
    /// class and be properly initialized with the associated <see cref="EntityController"/> and <see cref="Entity"/>.
    /// </param>
    /// <remarks>
    /// Note that the actual switch is delayed until the next tick to prevent an uninitialized behavior to be used.
    /// </remarks>
    public void SetBehavior(Behavior behavior)
    {
        CallDeferred("QueueBehaviorSwitch", behavior);
    }

    /// <summary>
    /// This method exists solely for use with Godot's <c>CallDeferred(string, Variant[])</c>,
    /// which only supports calling methods by name at runtime. (Why not use inline Actions?)
    /// 
    /// The behavior must be switched before the next physics frame. If not, the entity
    /// may operate with an uninitialized behavior, which causes runtime errors (non-fatal,
    /// but I just don't like errors duh).
    /// </summary>
    /// <param name="behavior">
    /// The new <see cref="Behavior"/> instance to apply. This becomes the entity's active behavior
    /// once the switch is performed.
    /// </param>
    private void QueueBehaviorSwitch(Behavior behavior)
    {
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

    public Vector3 Gravity()
    {
        return GravityMultiplier * Entity.GetGravity();
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
        
        return BaseSpeed ;
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
        // reaction behaviors can only be added with code. 
        // therefore, it cannot happen to accidentally use the same instance
        // TODO: Check if the resource has got a path. If not we can probably also skip the isolating
        if(behavior is ReactionBehavior) return behavior;
        
        if (_duplicated.Contains(behavior.ResourcePath))
        {
            Logger.Trace("{}: Behavior already isolated: [Type: {}]. Reusing existing instance.", Entity.Name, behavior.GetType().Name, behavior.ResourcePath);
            return behavior;
        }
        
        // I know, I know. Isolating sounds harsh. But they just WANT to be alone.
        Logger.Debug("{}: Isolate behavior: [Resource: {}].", Entity.Name, behavior.GetType().Name, behavior.ResourcePath);
        Behavior duplicated = (Behavior)behavior.Duplicate();
        _duplicated.Add(behavior.ResourcePath);
        
        return duplicated;
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