using System;
using Godot;

namespace RougeLiteGame.entity.limbs;

[GlobalClass]
public partial class Limb : RigidBody3D
{
	[Export(PropertyHint.Range, "0,10,0.5")] private float _lifeGain = 0.5f;
	[Export(PropertyHint.Range, "0,10,0.5")] private float _speedGain = 0.5f;
	[Export(PropertyHint.Range, "0,100,1")] private float _strengthGain = 10;

	public float Life { get; private set; }

	public float Speed { get; private set; }

	public float Strength { get; private set; }

	private Entity Host
	{
		get => _host;
		set
		{
			SetFreezeEnabled(value != null);	
			_host = value;
		}
	}

	private Entity _host;

	public override void _Ready()
	{
		Life = _lifeGain;
		Speed = _speedGain;
		_strengthGain = _lifeGain;
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
		Life = Math.Min(Life - damage, 0);
	}

	public void Heal(float heal)
	{
		Life = Math.Max(Life + heal, _lifeGain);
	}
}