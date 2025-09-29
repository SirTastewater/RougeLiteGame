using Godot;

namespace RougeLiteGame.entity.camera;

[GlobalClass]
public partial class CameraController : Node3D
{
    #region Attributes
    private bool _mouseInput;
    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    #endregion
    
    #region Mouse Settings
    [Export] private bool MouseInput
    {
        get => _mouseInput;
        set
        {
            _mouseInput = value;
            SetProcessInput(value);
        }
    }
    
    [Export(PropertyHint.Range, "0,10")] private float _mouseSensitivity = 5f;
    #endregion
    
    /**
     * If mouse input is enabled, the window will be capture the mouse.
     * Otherwise, why would it?
     */
    public override void _Ready()
    {
        SetProcessInput(_mouseInput);
        if (_mouseInput) Input.SetMouseMode(Input.MouseModeEnum.Captured);
        base._Ready();
    }

    /**
     * Listens to mouse movement and updates the camera rotation
     */
    public override void _Input(InputEvent @event)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        if (@event is not InputEventMouseMotion motionEvent) return;

        float actualSensitivity = _mouseSensitivity / 1000;

        Yaw -= motionEvent.Relative.X * actualSensitivity;
        Pitch -= motionEvent.Relative.Y * actualSensitivity;
        Pitch = Mathf.Clamp(Pitch, -1.4f, 1.4f);

        Rotation = new Vector3(Pitch, Yaw, Rotation.Z);

        base._Input(@event);
    }
}