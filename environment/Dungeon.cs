using System;
using System.Collections.Generic;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment;


public partial class Dungeon : Node
{
	private static readonly ILogger Logger = LoggerFactory.GetLogger<Dungeon>();
	[Export] private int mainPathLength = 5;
	[Export] private int gridSideLength = 11;

	private List<List<int>> grid = [];
	private List<DungeonNode> mainPath = [];
	private (int X, int Y)[] directions = { (1, 0), (0, 1), (-1, 0), (0, -1) };

	private struct DungeonNode(int x, int y)
	{
		public int X { get; } = x;
		public int Y { get; } = y;
		public int Connections { get; set; }
	}

	public override void _Ready()
	{
		for (int i = 0; i < gridSideLength; i++)
		{
			grid.Add([]);
			for (int j = 0; j < gridSideLength; j++)
			{
				grid[i].Add(0);
			}
		}

		int startCoordinate = gridSideLength / 2;
		DungeonNode startNode = new(startCoordinate, startCoordinate)
		{
			Connections = 1
		};
		grid[startCoordinate][startCoordinate] = 1;
		mainPath.Add(startNode);


		Random rnd = new Random();
		Tuple<int, int> lastPosition = new(startCoordinate, startCoordinate);

		

		for (int i = 1; i < mainPathLength; i++)
		{
			(int X, int Y)[] uncheckedDirections = directions;
			bool suitableTileFound;
			do
			{
				suitableTileFound = false;
				int tmp = rnd.Next(0, uncheckedDirections.Length);
				int tmpX = lastPosition.Item1 + uncheckedDirections[tmp].X;
				int tmpY = lastPosition.Item2 + uncheckedDirections[tmp].Y;

				var tmpList = new List<(int X, int Y)>(uncheckedDirections);
				tmpList.RemoveAt(tmp);
				uncheckedDirections = tmpList.ToArray();

				if (tmpX < gridSideLength && tmpY < gridSideLength)
				{
					if (grid[tmpX][tmpY] == 0 && !check_for_neighbour(tmpX,tmpY))
					{
						int connections = rnd.Next(2, 4);
						if (i == (mainPathLength - 1))
						{
							connections = 1;
						}
						DungeonNode tmpNode = new(tmpX, tmpY)
						{
							Connections = connections
						};
						grid[tmpX][tmpY] = connections;
						mainPath.Add(tmpNode);
						suitableTileFound = true;

						lastPosition = new(tmpX, tmpY);
					}
				}
			} while (!suitableTileFound);
		}

		for (int i = 0; i < grid.Count; i++) {
			String output = "";
			for (int j = 0; j < grid[i].Count; j++) {
				output += grid[i][j].ToString();
			}
			Logger.Info(output);
		}

    }


	private bool check_for_neighbour(int x, int y)
	{
		bool foundOneNeiggbour = false;

		foreach(var direction in directions)
		{
			int tmpX = x + direction.X;
			int tmpY = y + direction.Y;

			bool isInTheGrid = (tmpX < gridSideLength) && (tmpY < gridSideLength);

			if (isInTheGrid)
			{
				if(grid[tmpX][tmpY] != 0)
				{
					if (!foundOneNeiggbour)
					{
						foundOneNeiggbour = true;
					}
					else
					{
						return true;
					}
				}
			}
		}

		return false;
	}
}