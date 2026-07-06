using System;
using Godot;
using RougeLiteGame.environment.rooms;
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
    [ExportToolButton("Clear Dungeon")] private Callable ClearButton => Callable.From(Clear);

    private void Clear()
    {
        if (_gridMap == null)
        {
            Logger.Error("Dungeon: GridMap was not set");
            return;
        }
        
        _gridMap.Clear();
    }
    
    private void Generate()
    {
        if (_gridMap == null)
        {
            Logger.Error("Dungeon: GridMap was not set");
            return;
        }
        
        Clear();
        
        var pathGenerator = new PathGenerator(
            _pathLength,
            _startPosition,
            RandomNumberGenerator
        );

        Logger.Info("Generating Dungeon");
        Vector3I[] path = pathGenerator.GenerateMainPath();
        Logger.Info("Finished generating Dungeon-path {}.", path);

        for (int i = 0; i < path.Length; i++)
        {
            Vector3I vecI = path[i];
            IRoom room;
            if (i == 0)
            {
                room = new StartRoom(vecI,  path[i + 1]);
            }
            else if (i == path.Length - 1)
            {
                room = new EndRoom(vecI,  path[i - 1]);
            }
            else
            {
                Direction from = DirectionExtensions.GetDirectionByVectors(vecI, path[i - 1]);
                Direction to = DirectionExtensions.GetDirectionByVectors(vecI, path[i + 1]);

                if (from.IsOppositeOf(to))
                {
                    room = new StraightRoom(vecI, from, to);
                }
                else
                {
                    room = new CurvedRoom(vecI, from, to);
                }
            }
            
            _gridMap.SetCellItem(room.Location, room.AssetId, room.GetOrthogonalIndex(_gridMap));
        }

        
        Logger.Flush();
    }
    
    private static Direction GetDirectionByVectors(Vector3I from, Vector3I to)
    {
        Vector3I result = (to - from).Sign();
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