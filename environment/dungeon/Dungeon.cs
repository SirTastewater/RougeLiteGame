using System;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment.dungeon;

[Tool]
public partial class Dungeon : Node3D
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger<Dungeon>(false);
    private static readonly RandomNumberGenerator RandomNumberGenerator = new();

    [Export] private GridMap _gridMap;
    [Export] private int _pathLength = 3;
    [Export] private Vector3I _startPosition;


    [ExportToolButton("Generate Dungeon")] private Callable GenerateButton => Callable.From(Generate);

    private void Generate()
    {
        if (_gridMap == null)
        {
            Logger.Error("Dungeon: GridMap was not set");
            return;
        }
        
        _gridMap.Clear();
        var pathGenerator = new PathGenerator(
            _pathLength,
            _startPosition,
            RandomNumberGenerator
        );

        Logger.Info("Generating Dungeon");
        Vector3I[] path = pathGenerator.GenerateMainPath();
        Logger.Info("Finished generating Dungeon-path.");

        for (int i = 0; i < path.Length; i++)
        {
            var vecI = path[i];
            int roomIndex = 3;
            int rotation = 0;
            
            if (i == path.Length - 1)
            {
                roomIndex = 0;
                Direction direction = GetDirectionByVectors(vecI, path[i - 1]);
                Vector3I vector3I = direction.ToRotation();
                Basis basis = Basis.LookingAt(vector3I, Vector3I.Up);
                rotation = _gridMap.GetOrthogonalIndexFromBasis(basis);
            }
            _gridMap.SetCellItem(vecI, roomIndex, rotation);
        }

        Logger.Flush();
    }

    private static Direction GetDirectionByVectors(Vector3I from, Vector3I to)
    {
        Vector3I result = (from - to).Sign();
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            Vector2I vector2I = dir.ToVector();
            if (vector2I.X == result.X && vector2I.Y == result.Z)
            {
                return dir;
            }
        }

        return Direction.East;
    }
}