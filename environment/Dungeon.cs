using System.Collections.Generic;
using System.Linq;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment;


public partial class Dungeon : Node
{
	private static readonly ILogger Logger = LoggerFactory.GetLogger<Dungeon>();
	[Export] private int _mainPathLength = 5;
	[Export] private int _gridSideLength = 11;
	
	private RandomNumberGenerator _randomNumberGenerator = new();

	private int[][] _grid;
	private readonly List<DungeonNode> _mainPath = [];
	private readonly Vector2I[] _directions = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)];

	private struct DungeonNode(int x, int y)
	{
		public int X { get; } = x;
		public int Y { get; } = y;
		public int Connections { get; set; }
	}

	public override void _Ready()
	{
		_grid = new int[_gridSideLength][];
		for (int i = 0; i < _gridSideLength; i++)
		{
			_grid[i] = new int[_gridSideLength];
		}

		int startCoordinate = _gridSideLength / 2;
		DungeonNode startNode = new(startCoordinate, startCoordinate)
		{
			Connections = 1
		};
		_grid[startCoordinate][startCoordinate] = 1;
		_mainPath.Add(startNode);

		Vector2I lastPosition = new(startCoordinate, startCoordinate);

		for (int i = 1; i < _mainPathLength; i++)
		{
			List<Vector2I> uncheckedDirections = _directions.ToList();
			bool suitableTileFound;
			do
			{
				suitableTileFound = false;
				int tmp = _randomNumberGenerator.RandiRange(0, uncheckedDirections.Count - 1);
				int tmpX = lastPosition.X + uncheckedDirections[tmp].X;
				int tmpY = lastPosition.Y + uncheckedDirections[tmp].Y;

				uncheckedDirections.RemoveAt(tmp);

				if (tmpX >= _gridSideLength || tmpY >= _gridSideLength) continue;
				if (_grid[tmpX][tmpY] != 0 || CheckForNeighbour(tmpX, tmpY)) continue;
				
				int connections = _randomNumberGenerator.RandiRange(2, 4);
				if (i == _mainPathLength - 1)
				{
					connections = 1;
				}
				DungeonNode tmpNode = new(tmpX, tmpY)
				{
					Connections = connections
				};
				
				_grid[tmpX][tmpY] = connections;
				_mainPath.Add(tmpNode);
				suitableTileFound = true;

				lastPosition = new Vector2I(tmpX, tmpY);
			} while (!suitableTileFound);
		}

		foreach (var output in _grid.Select(item => item.Aggregate("", (current, t) => current + t)))
		{
			Logger.Info(output);
		}
    }

	private bool CheckForNeighbour(int x, int y)
	{
		bool foundOneNeighbour = false;

		foreach(Vector2I direction in _directions)
		{
			int tmpX = x + direction.X;
			int tmpY = y + direction.Y;

			bool isInTheGrid = tmpX < _gridSideLength && tmpY < _gridSideLength;

			if (!isInTheGrid) continue;
			if (_grid[tmpX][tmpY] == 0) continue;
			
			if (foundOneNeighbour)
			{
				return true;
			}
			
			foundOneNeighbour = true;
		}

		return false;
	}
}