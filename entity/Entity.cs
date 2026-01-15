using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity;

/// <summary>
///     Represents a base class for all game entities, inheriting from <see cref="Godot.CharacterBody3D" />.
/// </summary>
/// <remarks>
///     This abstract class serves as a common foundation for various entities within the game, such as players or NPCs.
/// </remarks>
// TODO: This class is currently just a placeholder and will need a lot of work still. Therefore this documentation is incomplete.
[GlobalClass]
public abstract partial class Entity : CharacterBody3D
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(Entity));
    [Export] private EntityController _entityController;

    public override void _Ready()
    {
        base._Ready();
        for (int i = 0; i < 1000000; i++)
        {
            Logger.Info("I like {} {} times 4", "trees", i);
        }
        _entityController?.Connect(this);
    }
}