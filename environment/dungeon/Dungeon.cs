using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment;


public partial class Dungeon : Node
{
	private static readonly ILogger Logger = LoggerFactory.GetLogger<Dungeon>();
	[Export] private int _mainPathLength = 5;
	[Export] private int _gridSideLength = 1000;
	[Export] private Node3D _roomContainer;
	private RandomNumberGenerator _randomNumberGenerator = new();

	private int[][] _grid;
	private readonly List<DungeonNode> _mainPath = [];
	private readonly Vector2I[] _directions = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)];
	enum DIRECTION {NORD, EAST, SOUTH, WEST, NONE}
	private int _roomLength = 6;
	private struct DungeonNode(int x, int y, DIRECTION nextRoomDirection, DIRECTION lastRoomDirection)
	{
		public int X { get; } = x;
		public int Y { get; } = y;
		public int Connections { get; set; }
		public DIRECTION NextRoomDirection { get; set; } = nextRoomDirection;
		public DIRECTION LastRoomDirection { get; set; } = lastRoomDirection;
	}

	public override void _Ready()
	{
		_grid = new int[_gridSideLength][];
		for (int i = 0; i < _gridSideLength; i++)
		{
			_grid[i] = new int[_gridSideLength];
		}

		int startCoordinate = _gridSideLength / 2;
		DungeonNode startNode = new(startCoordinate, startCoordinate, DIRECTION.NONE, DIRECTION.NONE)
		{
			Connections = 1
		};
		_grid[startCoordinate][startCoordinate] = 1;
		_mainPath.Add(startNode);

		DungeonNode lastNode = new(startCoordinate, startCoordinate, DIRECTION.NONE, DIRECTION.NONE);

		for (int i = 1; i < _mainPathLength; i++)
		{
			List<Vector2I> uncheckedDirections = _directions.ToList();
			bool suitableTileFound;
			do
			{
				suitableTileFound = false;
				int tmp = _randomNumberGenerator.RandiRange(0, uncheckedDirections.Count - 1);
				int tmpX = lastNode.X + uncheckedDirections[tmp].X;
				int tmpY = lastNode.Y + uncheckedDirections[tmp].Y;

				uncheckedDirections.RemoveAt(tmp);

				if (tmpX >= _gridSideLength || tmpY >= _gridSideLength) continue;
				if (_grid[tmpX][tmpY] != 0 || CheckForNeighbour(tmpX, tmpY)) continue;

				DungeonNode currentNode = new(tmpX, tmpY, DIRECTION.NONE, DIRECTION.NONE);

				if(uncheckedDirections[tmp].X == 0 && uncheckedDirections[tmp].Y == -1)
				{
					currentNode.LastRoomDirection = DIRECTION.SOUTH;
					lastNode.NextRoomDirection = DIRECTION.NORD;
				}
				else if(uncheckedDirections[tmp].X == 1 && uncheckedDirections[tmp].Y == 0)
				{
					currentNode.LastRoomDirection = DIRECTION.WEST;
					lastNode.NextRoomDirection = DIRECTION.EAST;
				}
				else if(uncheckedDirections[tmp].X == 0 && uncheckedDirections[tmp].Y == 1)
				{
					currentNode.LastRoomDirection = DIRECTION.NORD;
					lastNode.NextRoomDirection = DIRECTION.SOUTH;
				}
				else if(uncheckedDirections[tmp].X == -1 && uncheckedDirections[tmp].Y == 0)
				{
					currentNode.LastRoomDirection = DIRECTION.EAST;
					lastNode.NextRoomDirection = DIRECTION.WEST;
				}

				Logger.Info(currentNode.LastRoomDirection);
				Logger.Info(currentNode.NextRoomDirection);
				
				int connections = _randomNumberGenerator.RandiRange(2, 4);
				if (i == _mainPathLength - 1)
				{
					connections = 1;
				}
				DungeonNode tmpNode = new(tmpX, tmpY, DIRECTION.NONE, DIRECTION.NONE)
				{
					Connections = connections
				};
				
				_grid[tmpX][tmpY] = connections;
				_mainPath.Add(tmpNode);
				suitableTileFound = true;

				lastNode = currentNode;
			} while (!suitableTileFound);
		}

		bool isFirstRoom = true;

		string path;
		PackedScene packedScene;

		Room lastRoom = null;
		Room tmpRoom = null;

		do
		{
			DungeonNode currentNode = _mainPath.First<DungeonNode>();
			_mainPath.RemoveAt(0);

			if (isFirstRoom)
			{
				path = "res://environment/rooms/room_1/room_1.tscn";
				packedScene = GD.Load<PackedScene>(path);

				tmpRoom = packedScene.Instantiate<Room>();

				RotateRoomWithOneDoor(ref tmpRoom, ref currentNode);

				lastRoom = tmpRoom;
				_roomContainer.AddChild(tmpRoom);
				isFirstRoom = false;
			}
			else
			{
				switch (currentNode.Connections)
				{
					case 1:
						path = "res://environment/rooms/room_1/room_1.tscn";
						packedScene = GD.Load<PackedScene>(path);

						tmpRoom = packedScene.Instantiate<Room>();
						tmpRoom.Position = lastRoom.Position;

						Vector3 positionOffset = new(_roomLength,0,0);
						switch (currentNode.LastRoomDirection)
						{
							case DIRECTION.NORD:
								positionOffset = new(0,0,_roomLength);
								break;
							case DIRECTION.EAST:
								break;
							case DIRECTION.SOUTH:
								positionOffset = new(0,0,_roomLength * -1);
								break;
							case DIRECTION.WEST:
								positionOffset = new(_roomLength * -1,0,0);
								break;
						}
						tmpRoom.Position += positionOffset;

						RotateRoomWithOneDoor(ref tmpRoom, ref currentNode);
						break;
					case 2:
						break;
					case 3:
						break;
					case 4:
						path = "res://environment/rooms/room_4/room_4.tscn";
						packedScene = GD.Load<PackedScene>(path);

						tmpRoom = packedScene.Instantiate<Room>();
						break;
				}

				_roomContainer.AddChild(tmpRoom);
			}

			
		}while(_mainPath.Count > 0);
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


	private void RotateRoomWithOneDoor(ref Room tmpRoom, ref DungeonNode currentNode)
	{
		Vector3 rotationVector = new(0,270,0);
		DIRECTION rotationDirection;

		if((rotationDirection = currentNode.NextRoomDirection) == DIRECTION.NONE)
		{
			rotationDirection = currentNode.LastRoomDirection;
		}

		Logger.Info(currentNode.LastRoomDirection);
		Logger.Info(currentNode.NextRoomDirection);

		switch (rotationDirection)
		{
			case DIRECTION.NORD:
				rotationVector = new(0,0,0);
				break;
			case DIRECTION.EAST:
				break;
			case DIRECTION.SOUTH:
				rotationVector = new(0,180,0);
				break;
			case DIRECTION.WEST:
				rotationVector = new(0,90,0);
				break;
		}
		tmpRoom.RotationDegrees = rotationVector;
	}
}