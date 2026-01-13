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
	private bool _enableMouseInput;
	private Vector3 _relativePosition;
	#endregion
	
	#region Mouse Settings
	[Export] private bool EnableMouseInput { get => _enableMouseInput; set { _enableMouseInput = value; ConfigureMouseCapture(value); } }
	[Export(PropertyHint.Range, "0,10")] private float _mouseSensitivity = 5f;
	[Export] private Node3D _yawPivot;
	[Export] private Node3D _pitchPivot;
	#endregion
	
	public override void _Ready()
	{
		ConfigureMouseCapture(EnableMouseInput);
		
		_yawPivot ??= this;
		_pitchPivot ??= this;
		base._Ready();
	}

	public override void _Input(InputEvent @event)
	{
		if(Input.IsActionPressed("switch_input"))
		{
			EnableMouseInput = !EnableMouseInput;
		}
		
		if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
		if (@event is not InputEventMouseMotion motionEvent) return;
		
		float actualSensitivity = _mouseSensitivity / 1000;

		Yaw -= motionEvent.Relative.X * actualSensitivity;
		Pitch -= motionEvent.Relative.Y * actualSensitivity;
		Pitch = Mathf.Clamp(Pitch, -1.4f, 1.4f);
		
		_yawPivot.Rotation = new Vector3(_yawPivot.Rotation.X, Yaw, _yawPivot.Rotation.Z);
		_pitchPivot.Rotation = new Vector3(Pitch, _pitchPivot.Rotation.Y, _pitchPivot.Rotation.Z);

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
