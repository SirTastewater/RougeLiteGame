using System;
using System.Collections.Generic;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment.dungeon;

public class PathGenerator(int pathLength, Vector3I startPosition, RandomNumberGenerator numberGenerator)
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger<PathGenerator>(false);
    
    private Vector3I[] _path;
    private HashSet<Vector3I> _blockedTiles;

    public Vector3I[] GenerateMainPath()
    {
        SetupForGeneration();
        Vector3I currentPosition = startPosition;

        for (int i = 1; i < _path.Length; i++)
        {
            try
            {
                Vector3I resultPosition = GeneratePosition(currentPosition);
                
                currentPosition = resultPosition;
                _path[i] = resultPosition;
            }
            catch (PathStuckException pathStuckException)
            {
                i -= 2;
                currentPosition = _path[i];
                
                Logger.Warn("Path generation became stuck. Retry different route...", pathStuckException);
            }
        }
        
        Logger.Flush();
		
        return _path;
    }

    private void SetupForGeneration()
    {
        _path = new Vector3I[pathLength];
        _path[0] = startPosition;
        _blockedTiles = [startPosition];
    }
    
    private Vector3I GeneratePosition(Vector3I currentPosition)
    {
        List<Vector3I> candidates = GatherCandidates(currentPosition);
        
        if (candidates.Count == 0)
        {
            throw new PathStuckException();
        }
        
        Vector3I result = candidates[numberGenerator.RandiRange(0, candidates.Count - 1)];
        _blockedTiles.Add(result);
        return result; 
    }

    private List<Vector3I> GatherCandidates(Vector3I currentPosition)
    {
        List<Vector3I> candidates = [];
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            Vector3I resultPosition = currentPosition + ToVec3I(dir.ToVector());

            if (_blockedTiles.Contains(resultPosition))
            {
                continue;
            }

            candidates.Add(resultPosition);
        }
        
        return candidates;
    }

    private Vector3I ToVec3I(Vector2I vector2I)
    {
        return new Vector3I(vector2I.X, startPosition.Y, vector2I.Y);
    }
}