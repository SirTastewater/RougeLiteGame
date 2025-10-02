using Godot;
using RougeLiteGame.entity.behavior;
using RougeLiteGame.entity.behavior.idle;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity.player;

[GlobalClass]
public partial class PlayerBehavior : IdleBehavior, IInputAcceptor
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger<PlayerBehavior>();
    [Export] private bool _enableInput = true;
    [ExportGroup("Camera")]
    [Export(PropertyHint.NodePathValidTypes, "Node3D")] private NodePath _yawPivot;
    [Export(PropertyHint.NodePathValidTypes, "Node3D")] private NodePath _pitchPivot;
    [Export(PropertyHint.Range, "0,10")] private float _mouseSensitivity = 5f;
    
    
    private float _yaw, _pitch;
    private Node3D _yawPivotNode, _pitchPivotNode;
    
    protected override void Init()
    {
        _yawPivotNode = GetNode<Node3D>(_yawPivot);
        _pitchPivotNode = GetNode<Node3D>(_pitchPivot);
    }

    public override Vector3 Process(double delta)
    {
        if(!_enableInput) return Vector3.Zero;
        float speed = MovementSpeed();
        Vector3 velocity = Entity.Velocity;

        if (Input.IsActionJustPressed("jump") && Entity.IsOnFloor()) velocity.Y += Controller.JumpVelocity;
        
        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
        
        Vector3 direction = (Entity.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * speed;
            velocity.Z = direction.Z * speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Entity.Velocity.X, 0, speed);
            velocity.Z = Mathf.MoveToward(Entity.Velocity.Z, 0, speed);
        }

        return velocity;
    }

    public override bool IsSneaking()
    {
        return Input.IsActionPressed("sneak");
    }

    public  override bool IsSprinting()
    {
        return Input.IsActionPressed("sprint");
    }

    public void AcceptInput(InputEvent @event)
    {
        if(!_enableInput) return;
        
        if(Input.IsActionJustPressed("switch_input"))
        {
            bool isMouseCaptured = Input.GetMouseMode() == Input.MouseModeEnum.Captured;
            if (isMouseCaptured)
            {
                Logger.Debug("Releasing mouse capture.");
                Input.SetMouseMode(Input.MouseModeEnum.Visible);
                return;
            }
            
            Logger.Debug("Capturing mouse.");
            Input.SetMouseMode(Input.MouseModeEnum.Captured);
        }
        
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        if (@event is not InputEventMouseMotion motionEvent) return;
        
        float actualSensitivity = _mouseSensitivity / 1000;

        _yaw -= motionEvent.Relative.X * actualSensitivity;
        _pitch -= motionEvent.Relative.Y * actualSensitivity;
        _pitch = Mathf.Clamp(_pitch, -1.4f, 1.4f);
        
        _yawPivotNode.Rotation = new Vector3(_yawPivotNode.Rotation.X, _yaw, _yawPivotNode.Rotation.Z);
        _pitchPivotNode.Rotation = new Vector3(_pitch, _pitchPivotNode.Rotation.Y, _pitchPivotNode.Rotation.Z);
    }
}