using System;
using Godot;

namespace RougeLiteGame.entity.camera;

[GlobalClass]
public partial class CameraController : Node3D
{
    [Export] private bool _mouseInput;

    [Export(PropertyHint.Range, "0,10")] private float _mouseSensitivity = 5f;

    public float Yaw { get; private set; }
    private float Pitch { get; set; }

    public override void _Ready()
    {
        SetProcessInput(_mouseInput);
        if (_mouseInput)
        {
            Input.SetMouseMode(Input.MouseModeEnum.Captured);
        }
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