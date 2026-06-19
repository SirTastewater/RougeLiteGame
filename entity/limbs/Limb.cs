using System;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity.limbs;

[GlobalClass]
public partial class Limb : Node3D
{
	private static readonly ILogger logger = LoggerFactory.GetLogger<Limb>();
	
	[Export(PropertyHint.Range, "0,10,0.5")] private float _lifeGain = 0.5f;
	[Export(PropertyHint.Range, "0,10,0.5")] private float _speedGain = 0.5f;
	[Export(PropertyHint.Range, "0,100,1")] private float _strengthGain = 10;

	[Export] private PhysicalBoneSimulator3D _skeleton;
	
	public float Life { get; private set; }

	public float Speed
	{
		get
		{
			// easing function x² -> 50% life-loss leads to 75% loss of speed performance
			float value = (float) Math.Pow(Life / _lifeGain, 2f);
			return (float) Math.Ceiling(_speedGain * value * 100) / 100;
		}
	}

	public float Strength { get; private set; }

	private Entity Host
	{
		get => _host;
		set
		{
			if (value == null)
			{
				logger.Debug("Starting the simulation.");
				_skeleton.PhysicalBonesStartSimulation();
			}
			else
			{
				logger.Debug("Stopping the simulation.");
				_skeleton.PhysicalBonesStopSimulation();
			}
			_host = value;
		}
	}

	private Entity _host;

	public override void _Ready()
	{
		Life = _lifeGain;
		Speed = _speedGain;
		Strength = _strengthGain;
		
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