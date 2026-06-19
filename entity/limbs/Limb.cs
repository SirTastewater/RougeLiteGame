using System;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity.limbs;

[GlobalClass]
public partial class Limb : Node3D
{
	private static readonly ILogger Logger = LoggerFactory.GetLogger<Limb>();
	
	[Export(PropertyHint.Range, "0,10,0.5")] private float _lifeGain = 0.5f;
	[Export] private Curve SpeedCurve { get; set; }
	[Export] private Curve StrengthCurve { get; set; }
	
	[Export] private PhysicalBoneSimulator3D _skeleton;
	
	public float Life { get; private set; }

	public float Speed => SpeedCurve.Sample(SpeedCurve.MaxDomain / Math.Max(Life, 1)); // don't divide by 0
	public float Strength =>  StrengthCurve.Sample(StrengthCurve.MaxDomain / Math.Max(Life, 1));

	private Entity Host
	{
		get => _host;
		set
		{
			if (value == null)
			{
				Logger.Debug("Starting the simulation.");
				_skeleton.PhysicalBonesStartSimulation();
			}
			else
			{
				Logger.Debug("Stopping the simulation.");
				_skeleton.PhysicalBonesStopSimulation();
			}
			_host = value;
		}
	}

	private Entity _host;

	public override void _Ready()
	{
		Life = _lifeGain;
		SpeedCurve ??= new Curve();
		StrengthCurve ??= new Curve();
		
		Host = null;
	}

	public void Initialize(Entity host)
	{
		if (Host != null)
		{
			throw new Exception("Limb already initialized");
		}
		Host = host;
	}
	
	public void Uninitialize()
	{
		if (Host == null)
		{
			throw new Exception("Limb is not initialized");
		}
		
		Host = null;
	}
	
	public void Damage(float damage)
	{
		Life = Math.Max(Life - damage, 0);
	}

	public void Heal(float heal)
	{
		Life = Math.Min(Life + heal, _lifeGain);
	}
}