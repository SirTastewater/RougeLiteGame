using Godot;
using Godot.Collections;

namespace RougeLiteGame.entity;

[GlobalClass] public abstract partial class Entity : CharacterBody3D
{
    [Export] private EntityController _entityController;

    public override void _Ready()
    {
        base._Ready();
        
        _entityController.Connect(this);
    }
}