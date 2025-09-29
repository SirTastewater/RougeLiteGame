using System;
using Godot;

namespace RougeLiteGame.entity.camera;

[GlobalClass]
public partial class CameraPivot : Node3D
{
    [Export] private bool _mouseInput;

    [Export(PropertyHint.Range, "0,10")] private float _mouseSensitivity = 5f;
    private float _yaw, _pitch;
    
    public float Yaw => _yaw;
    public float Pitch => _pitch;
    
    public Action OnCameraMoved { get; set; }
    
    public override void _Ready()
    {
        SetProcessInput(_mouseInput);
        if (_mouseInput)
        {
            Input.SetMouseMode(Input.MouseModeEnum.Captured);
        }
        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        if (@event is not InputEventMouseMotion motionEvent) return;

        float actualSensitivity = _mouseSensitivity / 100;
        
        _yaw -= motionEvent.Relative.X * actualSensitivity;
        _pitch -= motionEvent.Relative.Y * actualSensitivity;
        _pitch = Mathf.Clamp(_pitch, -1.4f, 1.4f);
        
        Rotation = new Vector3(_pitch, _yaw, Rotation.Z);

        OnCameraMoved?.Invoke();
        
        base._Input(@event);
    }
}