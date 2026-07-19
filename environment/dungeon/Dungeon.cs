using System.Collections.Generic;
using Godot;
using RougeLiteGame.logger;
using RougeLiteGame.environment.rooms.room;
using RougeLiteGame.environment.rooms.room_2;
using RougeLiteGame.environment.rooms.room_3;
using RougeLiteGame.environment.rooms.room_1;
using RougeLiteGame.environment.rooms.corridor;

namespace RougeLiteGame.environment.dungeon;



public partial class Dungeon : Node
{
	private static readonly ILogger Logger = LoggerFactory.GetLogger<Dungeon>();
	[Export] private int _pathLength = 10;
	[Export] private int _maxSidePathLength = 3;
	[Export] private Node3D _roomContainer;
	[Export] private Node3D _corridorContainer;
	private List<Room> _path = [];
	private RandomNumberGenerator _randomNumberGenerator = new();
	private readonly List<Vector2I> _directions = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)];

	public override void _Ready()
	{
        GeneratePath();

		for(int i = 0; i < _path.Count; i++)
		{
			if(_path[i].Connections > 2)
			{
				_path[i].SideRoomDirection = GenerateSidePath(_path[i]);
			}
		}

		foreach(Room currentNode in _path)
		{
			_roomContainer.AddChild(currentNode);

			currentNode.Init();

			currentNode.Rotate();

			currentNode.UpdatePosition();
		}

		PrintPath();
	}

	private bool CheckIfPositionIsFree(int x,int y)
	{
		foreach(Room node in _path)
		{
			if(node.X == x && node.Y == y)
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckIfPositionIsSuitable(int x,int y)
	{
		Vector2I position = new(x,y);
		foreach(Vector2I direction in _directions)
		{
			Vector2I tmpPosition = position + direction;
			if(CheckIfPositionIsFree(tmpPosition.X,tmpPosition.Y))continue;
			return false;
		}
		return true;
	}

	private void GeneratePath()
	{
		Room startNode = new OneDoorRoom(0,0);

		Room lastNode = startNode;
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
				if(!CheckIfPositionIsSuitable(currentX,currentY)) continue;
				suitablePositionFound = true;

				lastNode.NextRoomDirection = GetDirectionFromVector(currentDirection);
				_path.Add(lastNode);

				int numberOfDoors = _randomNumberGenerator.RandiRange(2,4);

				Room currentNode = null;

				switch (numberOfDoors)
				{
					case 2:
						currentNode = new TwoDoorRoom(currentX,currentY);
						break;
					case 3:
						currentNode = new ThreeDoorRoom(currentX,currentY);
						break;
					case 4:
						currentNode = new FourDoorRoom(currentX,currentY);
						break;
				}

				currentNode.LastRoomDirection = GetOppositeDirection(lastNode.NextRoomDirection);
				
				lastNode = currentNode;
				lastX = currentX;
				lastY = currentY;

			} while (!suitablePositionFound);
		}

		DIRECTION tmp = lastNode.LastRoomDirection;

		lastNode = new OneDoorRoom(lastNode.X,lastNode.Y);
		lastNode.LastRoomDirection = tmp;
		
		
		_path.Add(lastNode);
	}

	private DIRECTION GenerateSidePath(Room startNode)
	{
		int numberOfSidePaths = startNode.Connections - 2;
		DIRECTION returnValue = DIRECTION.NONE;

		for(int i = 0; i < numberOfSidePaths; i++)
		{
			bool firstRun = true;
			int roomsPlaced = 0;
			Room lastNode = startNode;
			int lastX = startNode.X;
			int lastY = startNode.Y;

			for(int j = 0; j < _maxSidePathLength; j++)
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
					roomsPlaced++;

					if (!firstRun)
					{
						lastNode.NextRoomDirection = GetDirectionFromVector(currentDirection);
						_path.Add(lastNode);
					}
					else
					{
						returnValue = GetDirectionFromVector(currentDirection);
						firstRun = false;
					}

					Room currentNode = new TwoDoorRoom(currentX,currentY,true);

					currentNode.LastRoomDirection = GetOppositeDirection(GetDirectionFromVector(currentDirection));
					
					lastNode = currentNode;
					lastX = currentX;
					lastY = currentY;

				} while (!suitablePositionFound);
			}

			if(roomsPlaced != 0)
			{
				DIRECTION tmp = lastNode.LastRoomDirection;

				lastNode = new OneDoorRoom(lastNode.X,lastNode.Y,true);
				lastNode.LastRoomDirection = tmp;
				
				_path.Add(lastNode);
			}
		}
		return returnValue;
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
		foreach(Room tmp in _path)
		{
			Logger.Debug("({}, {}), connections: {}, next room: {}, last room: {}, side room {}", tmp.X, tmp.Y, tmp.Connections, tmp.NextRoomDirection, tmp.LastRoomDirection, tmp.SideRoomDirection);
		}
	}
}