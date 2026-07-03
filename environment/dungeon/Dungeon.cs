using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RougeLiteGame.logger;

namespace RougeLiteGame.environment.dungeon;


public partial class Dungeon : Node
{
	private static readonly ILogger Logger = LoggerFactory.GetLogger<Dungeon>();
	[Export] private int _pathLength = 2;
	[Export] private Node3D _roomContainer;
	private List<pathNode> _path = [];
	private RandomNumberGenerator _randomNumberGenerator = new();
	private readonly List<Vector2I> _directions = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)];
	enum DIRECTION {NORTH, EAST, SOUTH, WEST, NONE}
	private int _roomLength = 6;
	private struct pathNode(int x, int y)
	{
		public int X = x;
		public int Y = y;
		public int Connections;
		public DIRECTION NextRoomDirection = DIRECTION.NONE;
		public DIRECTION LastRoomDirection = DIRECTION.NONE;
	}

	public override void _Ready()
	{
        pathNode startNode = new(0, 0)
        {
            Connections = 1
        };

		pathNode lastNode = startNode;
		int lastX = 0;
		int lastY = 0;

		for(int i = 0; i < _pathLength -1; i++)
		{
			bool suitablePositionFound = false;
			List<Vector2I> uncheckedDirections = new(_directions);

			do
			{
				if(uncheckedDirections.Count == 0) break;

				int randomNumber = _randomNumberGenerator.RandiRange(0, uncheckedDirections.Count - 1);
				Vector2I currentDirection = uncheckedDirections[randomNumber];
				uncheckedDirections.RemoveAt(randomNumber);

				int currentX = lastX + currentDirection.X;
				int currentY = lastY + currentDirection.Y;

				if(!CheckIfPositionIsFree(currentX,currentY)) continue;
				suitablePositionFound = true;

				lastNode.NextRoomDirection = GetDirectionFromVector(currentDirection);
				_path.Add(lastNode);

				pathNode currentNode = new(currentX,currentY);

				currentNode.Connections = 2;

				currentNode.LastRoomDirection = GetOppositeDirection(lastNode.NextRoomDirection);
				
				lastNode = currentNode;
				lastX = currentX;
				lastY = currentY;

			} while (!suitablePositionFound);
		}

		lastNode.Connections = 1;
		
		_path.Add(lastNode);

		PrintPath();

		foreach(pathNode currentNode in _path)
		{
			Room room;
			Vector3 positionVector;
			switch (currentNode.Connections)
			{
				case 1:
					PackedScene oneDoorRoomScene = GD.Load<PackedScene>("res://environment/rooms/room_1/room_1.tscn");
					room = oneDoorRoomScene.Instantiate<Room>();

					DIRECTION doorDirection;
					
					if(currentNode.NextRoomDirection != DIRECTION.NONE)
					{
						doorDirection = currentNode.NextRoomDirection;
					}
					else
					{
						doorDirection = currentNode.LastRoomDirection;
					}

					RotateOneDoorRoom(doorDirection,ref room);

					positionVector = new(currentNode.X * _roomLength, 0, currentNode.Y * _roomLength);
					room.Position = positionVector;

					_roomContainer.AddChild(room);
					break;
				case 2: 
					if(RoomIsStraight(currentNode.LastRoomDirection, currentNode.NextRoomDirection))
					{
						PackedScene straightTwoDoorRoomScene = GD.Load<PackedScene>("res://environment/rooms/room_2/room_2_straight.tscn");
						room = straightTwoDoorRoomScene.Instantiate<Room>();

						RotateStraightRoom(currentNode.LastRoomDirection, ref room);
					}else
					{
						PackedScene curvedTwoDoorRoomScene = GD.Load<PackedScene>("res://environment/rooms/room_2/room_2_curve.tscn");
						room = curvedTwoDoorRoomScene.Instantiate<Room>();

						RotateCurvedRoom(currentNode.LastRoomDirection, currentNode.NextRoomDirection, ref room);
					}

					positionVector = new(currentNode.X * _roomLength, 0, currentNode.Y * _roomLength);
					room.Position = positionVector;

					_roomContainer.AddChild(room);
					break;
			}
		}
	}

	private bool CheckIfPositionIsFree(int x,int y)
	{
		foreach(pathNode node in _path)
		{
			if(node.X == x && node.Y == y)
			{
				return false;
			}
		}
		return true;
	}

	static private DIRECTION GetDirectionFromVector(Vector2I direction)
	{
		switch (direction)
		{
			case (1,0): 
				return DIRECTION.EAST;
			case (-1,0): 
				return DIRECTION.WEST;
			case (0,1): 
				return DIRECTION.SOUTH;
			case (0,-1): 
				return DIRECTION.NORTH;
			default:
				return DIRECTION.NONE;
		}
	}

	static private DIRECTION GetOppositeDirection(DIRECTION direction)
	{
		switch (direction)
		{
			case DIRECTION.WEST: 
				return DIRECTION.EAST;
			case DIRECTION.EAST: 
				return DIRECTION.WEST;
			case DIRECTION.NORTH: 
				return DIRECTION.SOUTH;
			case DIRECTION.SOUTH: 
				return DIRECTION.NORTH;
			default:
				return DIRECTION.NONE;
		}
	}

	private void PrintPath()
	{
		foreach(pathNode tmp in _path)
		{
			Logger.Debug("({}, {}), connections: {}, next room: {}, last room: {}", tmp.X, tmp.Y, tmp.Connections, tmp.NextRoomDirection, tmp.LastRoomDirection);
		}
	}

	static private void RotateOneDoorRoom(DIRECTION direction, ref Room room)
	{
		Vector3 rotationVector = new(0,0,0);

		switch (direction)
		{
			case DIRECTION.EAST:
				rotationVector.Y += 90;
				break;
			case DIRECTION.WEST:
				rotationVector.Y += 270;
				break;
			case DIRECTION.SOUTH:
				break;
			case DIRECTION.NORTH:
				rotationVector.Y += 180;
				break;
		}

		room.RotationDegrees = rotationVector;
	}

	static private bool RoomIsStraight(DIRECTION startDirection, DIRECTION endDirection)
	{
		switch (startDirection)
		{
			case DIRECTION.EAST when endDirection == DIRECTION.WEST:
			case DIRECTION.WEST when endDirection == DIRECTION.EAST:
			case DIRECTION.NORTH when endDirection == DIRECTION.SOUTH:
			case DIRECTION.SOUTH when endDirection == DIRECTION.NORTH:
				return true;
		}

		return false;
	}

	static private void RotateStraightRoom(DIRECTION direction, ref Room room)
	{
		Vector3 rotationVector = new(0,0,0);

		switch (direction)
		{
			case DIRECTION.EAST:
			case DIRECTION.WEST:
				rotationVector.Y += 90;
				break;
			case DIRECTION.SOUTH:
			case DIRECTION.NORTH:
				rotationVector.Y += 0;
				break;
		}

		room.RotationDegrees = rotationVector;
	}

	static private void RotateCurvedRoom(DIRECTION startDirection, DIRECTION endDirection, ref Room room)
	{
		Vector3 rotationVector = new(0,0,0);

		switch (startDirection)
		{
			case DIRECTION.EAST when endDirection == DIRECTION.NORTH:
				rotationVector.Y += 180;
				break;
			case DIRECTION.EAST when endDirection == DIRECTION.SOUTH:
				rotationVector.Y += 90;
				break;
			case DIRECTION.WEST when endDirection == DIRECTION.NORTH:
				rotationVector.Y += 270;
				break;
			case DIRECTION.WEST when endDirection == DIRECTION.SOUTH:
				rotationVector.Y += 0;
				break;
			case DIRECTION.SOUTH when endDirection == DIRECTION.EAST:
				rotationVector.Y += 90;
				break;
			case DIRECTION.SOUTH when endDirection == DIRECTION.WEST:
				rotationVector.Y += 0;
				break;
			case DIRECTION.NORTH when endDirection == DIRECTION.EAST:
				rotationVector.Y += 180;
				break;
			case DIRECTION.NORTH when endDirection == DIRECTION.WEST:
				rotationVector.Y += 270;
				break;
		}

		room.RotationDegrees = rotationVector;
	}
}