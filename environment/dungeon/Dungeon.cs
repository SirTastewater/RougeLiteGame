using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment.dungeon;


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

	private enum Direction {Nord, East, South, West, None}
	private int _roomLength = 6;
	private struct DungeonNode(int x, int y, Direction nextRoomDirection, Direction lastRoomDirection)
	{
		public int X { get; } = x;
		public int Y { get; } = y;
		public int Connections { get; set; }
		public Direction NextRoomDirection { get; set; } = nextRoomDirection;
		public Direction LastRoomDirection { get; set; } = lastRoomDirection;
	}

	public override void _Ready()
	{
		_grid = new int[_gridSideLength][];
		for (int i = 0; i < _gridSideLength; i++)
		{
			_grid[i] = new int[_gridSideLength];
		}

		int startCoordinate = _gridSideLength / 2;
		DungeonNode startNode = new(startCoordinate, startCoordinate, Direction.None, Direction.None)
		{
			Connections = 1
		};
		_grid[startCoordinate][startCoordinate] = 1;
		_mainPath.Add(startNode);

		for (int i = 0; i < _mainPathLength - 1; i++)
		{
			List<Vector2I> uncheckedDirections = _directions.ToList();
			bool suitableTileFound;
			do
			{
				suitableTileFound = false;
				int tmp = _randomNumberGenerator.RandiRange(0, uncheckedDirections.Count - 1);
				int tmpX = _mainPath[i].X + uncheckedDirections[tmp].X;
				int tmpY = _mainPath[i].Y + uncheckedDirections[tmp].Y;

				if (tmpX >= _gridSideLength || tmpY >= _gridSideLength) continue;
				if (_grid[tmpX][tmpY] != 0 || CheckForNeighbour(tmpX, tmpY)) continue;

				DungeonNode currentNode = new(tmpX, tmpY, Direction.None, Direction.None);

				switch (uncheckedDirections[tmp].X)
				{
					case 0 when uncheckedDirections[tmp].Y == -1:
					{
						currentNode.LastRoomDirection = Direction.South;
						DungeonNode tmpNode = _mainPath[i];
						tmpNode.NextRoomDirection = Direction.Nord;
						_mainPath[i] = tmpNode;
						break;
					}
					case 1 when uncheckedDirections[tmp].Y == 0:
					{
						currentNode.LastRoomDirection = Direction.West;
						DungeonNode tmpNode = _mainPath[i];
						tmpNode.NextRoomDirection = Direction.East;
						_mainPath[i] = tmpNode;
						break;
					}
					case 0 when uncheckedDirections[tmp].Y == 1:
					{
						currentNode.LastRoomDirection = Direction.Nord;
						DungeonNode tmpNode = _mainPath[i];
						tmpNode.NextRoomDirection = Direction.South;
						_mainPath[i] = tmpNode;
						break;
					}
					case -1 when uncheckedDirections[tmp].Y == 0:
					{
						currentNode.LastRoomDirection = Direction.East;
						DungeonNode tmpNode = _mainPath[i];
						tmpNode.NextRoomDirection = Direction.West;
						_mainPath[i] = tmpNode;
						break;
					}
				}

				uncheckedDirections.RemoveAt(tmp);
				
				int connections = _randomNumberGenerator.RandiRange(2, 4);
				if (i == _mainPathLength - 2)
				{
					currentNode.Connections = 1;
					connections = 1;
				}else
				{
					currentNode.Connections = connections;
				}
				
				_grid[tmpX][tmpY] = connections;
				_mainPath.Add(currentNode);
				suitableTileFound = true;
			} while (!suitableTileFound && uncheckedDirections.Count > 0);
		}

		bool isFirstRoom = true;

		string path;
		PackedScene packedScene;

		Room lastRoom = null;
		Room tmpRoom = null;

		do
		{
			DungeonNode currentNode = _mainPath.First();
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
						tmpRoom = PlaceRoomWithOneDoor(ref currentNode, ref lastRoom);
						break;
					case 2:
						tmpRoom = PlaceRoomWithTwoDoors(ref currentNode, ref lastRoom);
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


	private Room PlaceRoomWithOneDoor(ref DungeonNode currentNode, ref Room lastRoom)
	{
		const string path = "res://environment/rooms/room_1/room_1.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);

		Room tmpRoom = packedScene.Instantiate<Room>();
		tmpRoom.Position = lastRoom.Position;

		Vector3 positionOffset = new(_roomLength, 0, 0);
		switch (currentNode.LastRoomDirection)
		{
			case Direction.Nord:
				positionOffset.X = 0;
				positionOffset.Z = _roomLength;
				break;
			case Direction.South:
				positionOffset.X = 0;
				positionOffset.Z = _roomLength * -1;
				break;
			case Direction.West:
				positionOffset.X = _roomLength * -1;
				break;
			case Direction.East:
			case Direction.None: // fall-through
			default: break;
		}
		tmpRoom.Position += positionOffset;

		RotateRoomWithOneDoor(ref tmpRoom, ref currentNode);

		return tmpRoom;
	}


	private static void RotateRoomWithOneDoor(ref Room tmpRoom, ref DungeonNode currentNode)
	{
		Vector3 rotationVector = new(0,0,0);
		Direction rotationDirection;

		if((rotationDirection = currentNode.NextRoomDirection) == Direction.None)
		{
			rotationDirection = currentNode.LastRoomDirection;
		}

		float y = 0;
		switch (rotationDirection)
		{
			case Direction.Nord:
				y = 180;
				break;
			case Direction.East:
				y = 270;
				break;
			case Direction.West:
				y = 90;
				break;
			case Direction.South:
			case Direction.None: // fall-through
			default: break;
		}

		rotationVector.Y = y;
		tmpRoom.RotationDegrees = rotationVector;
	}


	private Room PlaceRoomWithTwoDoors(ref DungeonNode currentNode, ref Room lastRoom)
	{
		bool roomIsCurved = true;
		
		switch (currentNode.LastRoomDirection)
		{
			case Direction.Nord when currentNode.NextRoomDirection == Direction.South: // fall-through
			case Direction.South when currentNode.NextRoomDirection == Direction.Nord:
			case Direction.West when currentNode.NextRoomDirection == Direction.East:
			case Direction.East when currentNode.NextRoomDirection == Direction.West:
				roomIsCurved = false;
				break;
			case Direction.None: // fall-through
			default: break;
		}


		string roomName = roomIsCurved ? "room_2_curve.tscn" : "room_2_straight.tscn";
		string path = $"res://environment/rooms/room_2/{roomName}";

		PackedScene packedScene = GD.Load<PackedScene>(path);

		Room tmpRoom = packedScene.Instantiate<Room>();
		tmpRoom.Position = lastRoom.Position;

		Vector3 positionOffset = new(_roomLength,0,0);
		switch (currentNode.LastRoomDirection)
		{
			case Direction.Nord:
				positionOffset = new Vector3(0,0,_roomLength);
				break;
			case Direction.East:
				break;
			case Direction.South:
				positionOffset = new Vector3(0,0,_roomLength * -1);
				break;
			case Direction.West:
				positionOffset = new Vector3(_roomLength * -1,0,0);
				break;
			case Direction.None: // fall-through
			default: break;
		}
		tmpRoom.Position += positionOffset;

		RotateRoomWithOneDoor(ref tmpRoom, ref currentNode);

		return tmpRoom;
	}


	private void RotateRoomWithTwoDoors(ref Room tmpRoom, ref DungeonNode currentNode, bool roomIsCurved)
	{
		Vector3 rotationVector = new(0,0,0);

		if (roomIsCurved)
		{
			switch (currentNode.NextRoomDirection)
			{
				case DIRECTION.NORD:
					rotationVector = new(0,180,0);
					break;
				case DIRECTION.EAST:
					rotationVector = new(0,270,0);
					break;
				case DIRECTION.SOUTH:
					break;
				case DIRECTION.WEST:
					rotationVector = new(0,90,0);
					break;
			}
			tmpRoom.RotationDegrees = rotationVector;
		}else
		{
			switch (currentNode.NextRoomDirection)
			{
				case DIRECTION.NORD:
					break;
				case DIRECTION.EAST:
					rotationVector = new(0,90,0);
					break;
				case DIRECTION.SOUTH:
					break;
				case DIRECTION.WEST:
					rotationVector = new(0,90,0);
					break;
			}
			tmpRoom.RotationDegrees = rotationVector;
		}
	}
}