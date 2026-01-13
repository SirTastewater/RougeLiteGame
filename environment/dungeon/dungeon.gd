extends Node3D

#generation attributes
@export var NumberOfMiddleRooms : int = 5 

#container
@export var RoomContainer : Node3D
@export var CorridorContainer : Node3D


func _ready():
	Generate()


func Generate():
	var StartRoom = load("res://environment/endRoom/end_room_001.tscn").instantiate() as EndRoom
	
	RoomContainer.add_child(StartRoom)
	
	var NextPosition = StartRoom.EntranceMarker.global_position
	
	var Counter = 0
	
	while Counter < NumberOfMiddleRooms:
		var NextRoom = load("res://environment/middleRoom/middle_room_1_exit_001.tscn").instantiate() as MiddleRoom1Exit
		
		print(NextPosition)
		
		#only works if all entrances and exits are on the same y coordinate
		NextPosition += NextRoom.EntranceMarker.position
		
		NextRoom.global_position = NextPosition
		
		NextPosition += NextRoom.ExitMarker.global_position
		
		RoomContainer.add_child(NextRoom)
		
		Counter +=1
