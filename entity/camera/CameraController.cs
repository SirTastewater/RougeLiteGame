using System;
using Godot;

namespace RougeLiteGame.entity.camera;

/// <summary>
/// Controls the behavior and functionality of a 3D camera in relation to an actor.
/// The CameraController is responsible for updating the camera's orientation and handling mouse input to rotate the view.
/// </summary>
[GlobalClass]
public partial class CameraController : Node3D
{
    
    #region Attributes
    private float Yaw { get; set; }
    private float Pitch { get; set; }
    
    private bool _mouseInput;
    private Vector3 _relativePosition;
    public ICameraActor Actor { get; set; }
    #endregion
    
    #region Mouse Settings

    [Export]
    public bool MouseInput
    {
        get => _mouseInput; set 
        { 
            _mouseInput = value; 
            ConfigureMouseCapture(value); 
        }
    }
    
    [Export(PropertyHint.Range, "0,10")] private float _mouseSensitivity = 5f;
    #endregion
    
    public override void _Ready()
    {
        SetAsTopLevel(true);
        ConfigureMouseCapture(MouseInput);
        base._Ready();
    }

    public override void _Process(double delta)
    {
        Actor?.CameraProcess(this, Yaw, Pitch);
    }

    public override void _Input(InputEvent @event)
    {
        if(Input.IsActionPressed("switch_input"))
        {
            MouseInput = !MouseInput;
        }
        
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        if (@event is not InputEventMouseMotion motionEvent) return;
        
        float actualSensitivity = _mouseSensitivity / 1000;

        Yaw -= motionEvent.Relative.X * actualSensitivity;
        Pitch -= motionEvent.Relative.Y * actualSensitivity;
        Pitch = Mathf.Clamp(Pitch, -1.4f, 1.4f);

        Rotation = new Vector3(Pitch, Yaw, Rotation.Z);

        base._Input(@event);
    }

    /// <summary>
    /// Configures whether the mouse should be captured or visible
    /// </summary>
    /// <param name="value">
    /// If <c>true</c>, the mouse will be captured and hidden.
    /// If <c>false</c>, the mouse will be released and made visible.
    /// </param>
    /// <remarks>
    /// This method sets the input processing state and updates the mouse mode accordingly:
    /// <list type="bullet">
    /// <item><description><see cref="Input.MouseModeEnum.Captured"/> locks and hides the cursor.</description></item>
    /// <item><description><see cref="Input.MouseModeEnum.Visible"/> shows and frees the cursor.</description></item>
    /// </list>
    /// </remarks>
    private static /* unstatic later */ void ConfigureMouseCapture(bool value)
    {
        // SetProcessInput(value); // TODO: Disabled for debugging purposes
        if (value)
        {
            Input.SetMouseMode(Input.MouseModeEnum.Captured);
            return;
        }
        
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }
}