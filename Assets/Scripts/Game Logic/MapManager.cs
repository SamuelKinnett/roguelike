﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{

	MapTile[,] map;
	//This 2D array contains all of the map tiles
	List<MapTile> emptyTiles;
	//A list of all non-collideable (i.e. empty) tiles
	List<MapTile> spawnPoints;
	//A list of all non-cllideable tiles surrounded by non-collideable tiles on all sides,
	// ideal spawn points for enemies or staircases.
	int[] stairPositions;
	//Contains the x and y of the up stairs, then the x and y of the down stairs
	public int mapWidth, mapHeight;
	public int maxRoomWidth, maxRoomHeight;
	public string paletteName;

	public int viewRange;
	public float viewFalloff;
	public bool falloffEnabled;

	Dungeon dungeon;

	//TESTING
	public Sprite testingSprite;
	public Sprite upStair;
	public Sprite downStair;
	public int rayDivisor;
	bool wew = false;

	// Use this for initialization
	void Start ()
	{
		//clamp the falloff value such that the "light level" of a tile
		// never falls below 0.2f

		if (viewFalloff * viewRange > 0.8f) {
			viewFalloff = 0.8f / viewRange;
		}
	}

	/// <summary>
	/// Returns a TileInfo structure for the tile at the specified position
	/// </summary>
	public TileInfo GetTile (int x, int y)
	{
		return map [x, y].GetInfo ();
	}

	/// <summary>
	/// Returns the x and y co-ordinate of a pseudo-random non-collidable tile 
	/// </summary>
	public int[] GetRandomPosition (bool spawnPoint = true)
	{
		//Create and instantiate a new random number generator
		System.Random rand = new System.Random ();

		int[] returnArray = new int[2];

		if (!spawnPoint) {
			//Return the index of a random empty tile
			int returnTileIndex = (int)((emptyTiles.Count - 1) * rand.NextDouble ()); 
			returnArray [0] = emptyTiles [returnTileIndex].GetInfo ().x;
			returnArray [1] = emptyTiles [returnTileIndex].GetInfo ().y;
		} else {
			//Return the index of a random spawn point
			int returnTileIndex = (int)((spawnPoints.Count - 1) * rand.NextDouble ()); 
			returnArray [0] = spawnPoints [returnTileIndex].GetInfo ().x;
			returnArray [1] = spawnPoints [returnTileIndex].GetInfo ().y;
		}


		return returnArray;
	}

	/// <summary>
	/// Populates a list of all non-collideable tiles.
	/// </summary>
	void UpdateEmptyTileList ()
	{
		emptyTiles = new List<MapTile> ();
		spawnPoints = new List<MapTile> ();

		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				if (map [x, y] != null && map [x, y].GetInfo ().active) {
					if (!map [x, y].GetInfo ().solid)
						emptyTiles.Add (map [x, y]);
				}
			}
		}

		for (int x = 1; x < mapWidth - 1; ++x) {
			for (int y = 1; y < mapHeight - 1; ++y) {
				int surroundings = 0;
				if (map [x - 1, y] != null && map [x - 1, y].GetInfo ().active && !map [x - 1, y].GetInfo ().solid)
					surroundings = surroundings | 1;
				if (map [x + 1, y] != null && map [x + 1, y].GetInfo ().active && !map [x + 1, y].GetInfo ().solid)
					surroundings = surroundings | 2;
				if (map [x, y - 1] != null && map [x, y - 1].GetInfo ().active && !map [x, y - 1].GetInfo ().solid)
					surroundings = surroundings | 4;
				if (map [x, y + 1] != null && map [x, y + 1].GetInfo ().active && !map [x, y + 1].GetInfo ().solid)
					surroundings = surroundings | 8;
				if (map [x - 1, y - 1] != null && map [x - 1, y - 1].GetInfo ().active && !map [x - 1, y - 1].GetInfo ().solid)
					surroundings = surroundings | 16;
				if (map [x + 1, y - 1] != null && map [x + 1, y - 1].GetInfo ().active && !map [x + 1, y - 1].GetInfo ().solid)
					surroundings = surroundings | 32;
				if (map [x - 1, y + 1] != null && map [x - 1, y + 1].GetInfo ().active && !map [x - 1, y + 1].GetInfo ().solid)
					surroundings = surroundings | 64;
				if (map [x + 1, y + 1] != null && map [x + 1, y + 1].GetInfo ().active && !map [x + 1, y + 1].GetInfo ().solid)
					surroundings = surroundings | 128;

				if (surroundings == 255)
					spawnPoints.Add (map [x, y]);
			}
		}
	}

	public void InitialiseMap ()
	{
		map = new MapTile[mapWidth, mapHeight];

		Debug.Log ("Map Initialised");

		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				map [x, y] = new MapTile ();
			}
		}
	}

	public void RecalculateFogOfWar (int playerX, int playerY)
	{
		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				if (map [x, y] != null) {
					if (GetTile (x, y).visibility == TileVisibility.visible
					    && GetTile (x, y).active) {
						map [x, y].SetVisibility (TileVisibility.seen);
					}
				}
			}
		}
		for (int angle = -180; angle < 180; angle += rayDivisor) {
			float xDisplacement = Mathf.Cos (angle * Mathf.Deg2Rad);
			float yDisplacement = Mathf.Sin (angle * Mathf.Deg2Rad);
			float tempX = playerX;
			float tempY = playerY;
			int mapX = playerX;
			int mapY = playerY;
			int count = 0;

			while (!map [mapX, mapY].GetInfo ().solid
			       && map [mapX, mapY].GetInfo ().active
			       && count < viewRange) {
				if (falloffEnabled) {
					map [mapX, mapY].SetLightIntensity ((float)(1.0f - (viewFalloff * count)));
					map [mapX, mapY].SetVisibility (TileVisibility.visible);
				} else
					map [mapX, mapY].SetVisibility (TileVisibility.visible);
				tempX += xDisplacement;
				tempY += yDisplacement;
				mapX = (int)Mathf.Round (tempX);
				mapY = (int)Mathf.Round (tempY);
				++count;

				if (mapX >= mapWidth || mapY >= mapHeight || mapX < 0 || mapY < 0)
					break;
			}

			//Now, make sure that the wall tile is visible
			if (mapX < mapWidth && mapY < mapHeight
			    && mapX >= 0 && mapY >= 0) {
				if (falloffEnabled) {
					map [mapX, mapY].SetLightIntensity ((float)(1.0f - (viewFalloff * count)));
					map [mapX, mapY].SetVisibility (TileVisibility.visible);
				} else
					map [mapX, mapY].SetVisibility (TileVisibility.visible);
			}
		}
	}

	#region Map Generation

	/// <summary>
	/// This class models a "chunk" of dungeon
	/// </summary>
	class MapNode
	{

		//The x and y co-ordinates refer to the bottom left corner of the region.
		int x, y, width, height, depth;
		int splitType;
		//0 = horizontal, 1 = vertical
		MapNode[] children;

		public MapNode (int x, int y, int width, int height, int depth)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.depth = depth;
		}

		/// <summary>
		/// Recursively split the map down into chunks, stored as a tree
		/// </summary>
		/// <param name="desiredDepth">Desired depth.</param>
		public void Split (int desiredDepth)
		{

			if (depth == desiredDepth
			    || width < 10
			    || height < 10)
				return;
			
			double splitAmount;

			children = new MapNode[2];

			//The split percentage should be between 40% and 60% of the parent node
			splitAmount = 0.2f * Random.value + 0.4f;

			//randomly choose an axis to split accross
			int axis = (int)(100 * Random.value);
			if (axis % 2 == 1) {
				//split horizontally
				splitType = 0;

				//We now need to ensure that no room is ever split such that the resultant rooms
				// have a width or height smaller than 5, since the smallest possible dungeon
				// room is 3x3 and we want to be able to place this comfortably with a 1 block
				// border.

				int splitLocation = (int)(height * splitAmount);
				while (splitLocation < 5)
					++splitLocation;
				while (height - splitLocation < 5)
					--splitLocation;
				
				children [0] = new MapNode (x, y, width, splitLocation, depth + 1);
				children [0].Split (desiredDepth);
				children [1] = new MapNode (x, y + splitLocation, width, height - splitLocation, depth + 1);
				children [1].Split (desiredDepth);
			} else {
				//split vertically
				splitType = 1;
				int splitLocation = (int)(width * splitAmount);
				while (splitLocation < 5)
					++splitLocation;
				while (height - splitLocation < 5)
					--splitLocation;
				
				children [0] = new MapNode (x, y, splitLocation, height, depth + 1);
				children [0].Split (desiredDepth);
				children [1] = new MapNode (x + splitLocation, y, width - splitLocation, height, depth + 1);
				children [1].Split (desiredDepth);
			}
		}

		/// <summary>
		/// Places rooms randomly at the specified depth and connects them with
		/// corridors.
		/// Returns a the updated collision map
		/// </summary>
		public int[,] PlaceRooms (int desiredDepth, int[,] collisionMap, int maxRoomWidth, int maxRoomHeight)
		{
			int[,] tempCollisionMap = collisionMap;

			if (children == null) {

				//Decide on the position of the room and then the dimensions
				//Place the origin of the room somewhere within the lower left corner
				//of the region.
				int roomX = (int)(((width / 2) * Random.value) + x);
				int roomY = (int)(((height / 2) * Random.value) + y);

				//Ensure that the room never touches the area boundaries
				if (roomX == x)
					++roomX;

				if (roomY == y)
					++roomY;

				int newMaxWidth; // = width - (roomX - x);
				int newMaxHeight; // = height - (roomY - y);

				//If the room is smaller than the maximum room size specified, then the maximum
				// width the generated room can be is as big as the room size. Otherwise, we
				// clamp the value.

				if (width < maxRoomWidth)
					newMaxWidth = width - 2;
				else
					newMaxWidth = maxRoomWidth - 2;

				if (height < maxRoomHeight)
					newMaxHeight = height - 2;
				else
					newMaxHeight = maxRoomHeight - 2;

				int roomWidth = (int)((newMaxWidth - 3) * Random.value + 3);
				int roomHeight = (int)((newMaxHeight - 3) * Random.value + 3);

				//Clamp the width and height values
				if (roomWidth > maxRoomWidth)
					roomWidth = maxRoomWidth;

				if (roomHeight > maxRoomHeight)
					roomHeight = maxRoomHeight;

				//Ensure that the room never touches the area boundaries

				while (roomX + roomWidth >= x + width)
					--roomX;

				while (roomY + roomHeight >= y + height)
					--roomY;

				for (int tempX = roomX; tempX < roomX + roomWidth; ++tempX) {
					for (int tempY = roomY; tempY < roomY + roomHeight; ++tempY) {
						tempCollisionMap [tempX, tempY] = 1;	//Make this area not solid.
					}
				}
			} else {
				//Call the function recursively
				tempCollisionMap = children [0].PlaceRooms (desiredDepth, tempCollisionMap, maxRoomWidth, maxRoomHeight);
				tempCollisionMap = children [1].PlaceRooms (desiredDepth, tempCollisionMap, maxRoomWidth, maxRoomHeight);
				//After this, link the two with a corridor.
				tempCollisionMap = JoinRooms (children [0], children [1], tempCollisionMap);
			}

			return tempCollisionMap;
		}

		/// <summary>
		/// Connects two rooms or regions with a corridor
		/// It's important to note that die to the way splitting is handled, the first
		/// room will always be below or to the left of the second room.
		/// </summary>
		int[,] JoinRooms (MapNode room1, MapNode room2, int[,] collisionMap)
		{
			if (splitType == 0) {
				//The rooms have been split horizontally, so we need a vertical line.

				//Firstly, find a line halfway between the edges of the two rooms
				//well, near-as-damnit anyway.

				int topRoomY, bottomRoomY;
				bool spaceFound = false;

				//Assign values to topRoomY and bottomRoomY to shut up the compiler
				topRoomY = -1;
				bottomRoomY = -1;

				//We'll need to start by finding the y value of the two edges. Look from
				//top to bottom so that we know the first space encountered will be the
				//"highest" empty point in the lower area.
				for (int tempY = room1.y + room1.height - 1; tempY >= room1.y; --tempY) {
					for (int tempX = room1.x; tempX < room1.x + room1.width; ++tempX) {
						if (collisionMap [tempX, tempY] == 1) {
							//We've found empty space
							bottomRoomY = tempY;
							spaceFound = true;
							break;
						}
					}
					if (spaceFound)
						break;
				}

				if (!spaceFound)
					Debug.Log ("Oh snap, error: " + room1.x + ", " + room1.y + ", " + room1.width + ", " + room1.height);

				//Now, calculate the "lowest" y value of the top area
				spaceFound = false;
				for (int tempY = room2.y; tempY < room2.y + room2.height; ++tempY) {
					for (int tempX = room2.x; tempX < room2.x + room2.width; ++tempX) {
						if (collisionMap [tempX, tempY] == 1) {
							//We've found empty space
							topRoomY = tempY;
							spaceFound = true;
							break;
						}
					}
					if (spaceFound)
						break;
				}
					
				//Now we can calculate the midpoint
				int midpointY = (int)((bottomRoomY + topRoomY) / 2);

				//Next, decide on a point along the bottom of the top area, and the top of the bottom area
				// as end points of the corridor.

				//Rather than choosing from all positions randomly, then checking if they're inside the room,
				// we'll first populate two lists which we can randomly select from.
				List<int> possibleTopX = new List<int> ();
				List<int> possibleBottomX = new List<int> ();

				for (int tempX = room1.x; tempX < room1.x + (room1.width - 1); ++tempX) {
					if (collisionMap [tempX, bottomRoomY] == 1) {
						possibleBottomX.Add (tempX);
					}
				}
					
				for (int tempX = room2.x; tempX < room2.x + (room2.width - 1); ++tempX) {
					if (collisionMap [tempX, topRoomY] == 1) {
						possibleTopX.Add (tempX);
					}
				}

				//TODO: add code here to check if a straight corridor is possible and, if it is, place it
				// instead of an 's' shaped corridor.

				int endPointTopX, endPointBottomX;
				endPointTopX = possibleTopX [(int)((possibleTopX.Count - 1) * Random.value)];
				endPointBottomX = possibleBottomX [(int)((possibleBottomX.Count - 1) * Random.value)];

				//Place the corridor
				for (int tempY = topRoomY; tempY >= midpointY; --tempY) {
					collisionMap [endPointTopX, tempY] = 1;
				}

				if (endPointTopX < endPointBottomX) {
					for (int tempX = endPointTopX; tempX <= endPointBottomX; ++tempX) {
						collisionMap [tempX, midpointY] = 1;
					}
				} else {
					for (int tempX = endPointTopX; tempX >= endPointBottomX; --tempX) {
						collisionMap [tempX, midpointY] = 1;
					}
				}

				for (int tempY = bottomRoomY; tempY <= midpointY; ++tempY) {
					collisionMap [endPointBottomX, tempY] = 1;
				}

			} else {
				//The rooms have been split vertically, so we need a horizontal line.

				//Firstly, find a line halfway between the edges of the two rooms
				//well, near-as-damnit anyway.

				int rightRoomX, leftRoomX;
				bool spaceFound = false;

				//Assign values to rightRoomX and leftRoomX to shut up the compiler
				rightRoomX = -1;
				leftRoomX = -1;

				//We'll need to start by finding the x value of the two edges. Look from
				//right to left so that we know the first space encountered will be the
				//rightmost empty point in the left area.
				for (int tempX = room1.x + room1.width - 1; tempX >= room1.x; --tempX) {
					for (int tempY = room1.y; tempY < room1.y + room1.height; ++tempY) {
						if (collisionMap [tempX, tempY] == 1) {
							//We've found empty space
							leftRoomX = tempX;
							spaceFound = true;
							break;
						}
					}
					if (spaceFound)
						break;
				}

				//Now, calculate the leftmost x value of the right area
				spaceFound = false;
				for (int tempX = room2.x; tempX < room2.x + room2.width; ++tempX) {
					for (int tempY = room2.y; tempY < room2.y + room2.height; ++tempY) {
						if (collisionMap [tempX, tempY] == 1) {
							//We've found empty space
							rightRoomX = tempX;
							spaceFound = true;
							break;
						}
					}
					if (spaceFound)
						break;
				}

				//Now we can calculate the midpoint
				int midpointX = (int)((leftRoomX + rightRoomX) / 2);

				//Next, decide on a point along the left of the right area, and the right of the left area
				// as end points of the corridor.

				//Rather than choosing from all positions randomly, then checking if they're inside the room,
				// we'll first populate two lists which we can randomly select from.
				List<int> possibleRightY = new List<int> ();
				List<int> possibleLeftY = new List<int> ();

				//Debug.Log ("Beginning Left Search! Room co-ords: " + room1.x + ", " + room1.y +
				//"\nRoom dimensions: " + room1.width + ", " + room1.height);
				for (int tempY = room1.y; tempY < (room1.y + room1.height - 1); ++tempY) {
					if (collisionMap [leftRoomX, tempY] == 1) {
						possibleLeftY.Add (tempY);
					}
				}

				for (int tempY = room2.y; tempY < (room2.y + room2.height - 1); ++tempY) {
					if (collisionMap [rightRoomX, tempY] == 1) {
						possibleRightY.Add (tempY);
					}
				}

				//TODO: add code here to check if a straight corridor is possible and, if it is, place it
				// instead of an 's' shaped corridor.

				int endPointRightY, endPointLeftY;
				endPointRightY = possibleRightY [(int)((possibleRightY.Count - 1) * Random.value)];
				endPointLeftY = possibleLeftY [(int)((possibleLeftY.Count - 1) * Random.value)];

				//Place the corridor
				for (int tempX = leftRoomX; tempX <= midpointX; ++tempX) {
					collisionMap [tempX, endPointLeftY] = 1;
				}

				if (endPointRightY < endPointLeftY) {
					for (int tempY = endPointRightY; tempY <= endPointLeftY; ++tempY) {
						collisionMap [midpointX, tempY] = 1;
					}
				} else {
					for (int tempY = endPointLeftY; tempY <= endPointRightY; ++tempY) {
						collisionMap [midpointX, tempY] = 1;
					}
				}

				for (int tempX = midpointX; tempX <= rightRoomX; ++tempX) {
					collisionMap [tempX, endPointRightY] = 1;
				}
			}
			return collisionMap;
		}
	}

	public void GenerateDungeon (int mapWidth, int mapHeight, int numberOfFloors)
	{
		dungeon = new Dungeon ();
		this.mapWidth = mapWidth;
		this.mapHeight = mapHeight;
		InitialiseMap ();

		for (int floor = 0; floor < numberOfFloors - 1; ++floor) {
			GenerateFloor (mapWidth, mapHeight);
		}
		GenerateFloor (mapWidth, mapHeight, true);
	}

	/// <summary>
	/// Called to pseudo-randomly generate a map.
	/// Returns the co-ordinates of the starting stairs
	/// </summary>
	public void GenerateFloor (int mapWidth, int mapHeight, bool bottomFloor = false)
	{
		int[,] collisionMap = new int[mapWidth, mapHeight];
		MapNode mapGenerator = new MapNode (0, 0, mapWidth, mapHeight, 0);

		mapGenerator.Split (4);	//This will split the map 4 times

		//Now, generate the collision map
		collisionMap = mapGenerator.PlaceRooms (4, collisionMap, maxRoomWidth, maxRoomHeight);

		//TODO: add code here to place doors and subsequently
		// distinguish between rooms and corridors

		stairPositions = PlaceStairs (collisionMap);

		dungeon.AddFloor (mapWidth, mapHeight, collisionMap, stairPositions, bottomFloor);
	}

	/// <summary>
	/// Loads the map at the specified level
	/// </summary>
	/// <returns>The map.</returns>
	/// <param name="level">Level.</param>
	public int[] LoadMap (int level, bool returning = false)
	{
		int[,] collisionMap = new int[mapWidth, mapHeight];

		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				if (map [x, y].GetInfo ().active)
					map [x, y].SetInactive ();
			}
		}

		//Clear the current maptile gameobjects
		/*
		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				if (map [x, y] != null) {
					map [x, y].DestroyTile ();
					Destroy (map [x, y]);
				}
			}
		}
		*/
			
		stairPositions = dungeon.GetFloor (level).GetStairPositions ();
		CreateTiles (dungeon.GetFloor (level).GetCollisionMap (),
			dungeon.GetFloor (level).GetVisibilityMap ());

		UpdateEmptyTileList ();

		int[] returnPositions = new int[2];

		if (returning) {
			//The player has gone up the stairs
			returnPositions [0] = stairPositions [2];
			returnPositions [1] = stairPositions [3];
		} else {
			returnPositions [0] = stairPositions [0];
			returnPositions [1] = stairPositions [1];
		}

		return returnPositions;
	}

	/// <summary>
	/// Any variables that need to be saved to a particular level go here.
	/// </summary>
	/// <param name="level">Level.</param>
	public void SaveMap (int level)
	{
		TileVisibility[,] visibilityMap = new TileVisibility[mapWidth, mapHeight];
		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				if (map [x, y] != null && map [x, y].GetInfo ().active) {
					visibilityMap [x, y] = map [x, y].GetInfo ().visibility;
				}
			}
		}

		dungeon.SetVisibilityMap (visibilityMap, level);
	}

	/// <summary>
	/// Creates tile instances and intelligently assigns them sprites
	/// </summary>
	//TODO: Implement a way of distinguishing between rooms and corridors
	void CreateTiles (int[,] collisionMap, TileVisibility[,] visibilityMap)
	{

		//Load tile palette;
		GameObject paletteObject = (GameObject)Resources.Load ("Palettes/" + paletteName);
		MapPalette palette = paletteObject.GetComponent<MapPalette> ();

		Sprite tileSprite = testingSprite;
		int[,] wallMap = new int[mapWidth, mapHeight];	//Used to place walls more easily

		//Place the floor tiles
		for (int tempX = 0; tempX < mapWidth; ++tempX) {
			for (int tempY = 0; tempY < mapHeight; ++tempY) {
				if (tempX == stairPositions [0] &&
				    tempY == stairPositions [1]) {
					//This tile is the upwards staircase
					map [tempX, tempY].SetTile (tempX, tempY, false, true, false, upStair);
					map [tempX, tempY].SetVisibility (visibilityMap [tempX, tempY]);
				} else if (tempX == stairPositions [2] &&
				           tempY == stairPositions [3]) {
					//This tile is the downwards staircase
					map [tempX, tempY].SetTile (tempX, tempY, false, false, true, downStair);
					map [tempX, tempY].SetVisibility (visibilityMap [tempX, tempY]);
				} else {
					if (collisionMap [tempX, tempY] == 1) {
						//using bits as flags, work out the index of the sprite for the tile
						int surroundingTiles = 0;

						if (tempX > 0)
						if (collisionMap [tempX - 1, tempY] == 0)
							surroundingTiles = surroundingTiles | 1;

						if (tempY < mapHeight - 1)
						if (collisionMap [tempX, tempY + 1] == 0)
							surroundingTiles = surroundingTiles | 2;

						if (tempX < mapWidth - 1)
						if (collisionMap [tempX + 1, tempY] == 0)
							surroundingTiles = surroundingTiles | 4;

						if (tempY > 0)
						if (collisionMap [tempX, tempY - 1] == 0)
							surroundingTiles = surroundingTiles | 8;

						switch (surroundingTiles) {
						case 0:
						//No surrounding walls
							tileSprite = palette.floorTiles [(int)FloorPalette.Centre];
							break;
						case 1:
						//Wall to the left
							tileSprite = palette.floorTiles [(int)FloorPalette.Left];
							break;
						case 2:
						//Wall to the top
							tileSprite = palette.floorTiles [(int)FloorPalette.Top];
							break;
						case 3:
						//Walls to the left and top
							tileSprite = palette.floorTiles [(int)FloorPalette.TopLeft];
							break;
						case 4:
						//Wall to the right
							tileSprite = palette.floorTiles [(int)FloorPalette.Right];
							break;
						case 5:
						//Walls to the left and right
							tileSprite = palette.floorTiles [(int)FloorPalette.Vertical];
							break;
						case 6:
						//Walls to the top and right
							tileSprite = palette.floorTiles [(int)FloorPalette.TopRight];
							break;
						case 7:
						//Walls to the top, left and right
							tileSprite = palette.floorTiles [(int)FloorPalette.EndPieceTop];
							break;
						case 8:
						//Wall to the bottom
							tileSprite = palette.floorTiles [(int)FloorPalette.Bottom];
							break;
						case 9:
						//Walls to the bottom and left
							tileSprite = palette.floorTiles [(int)FloorPalette.BottomLeft];
							break;
						case 10:
						//Walls to the bottom and top
							tileSprite = palette.floorTiles [(int)FloorPalette.Horizontal];
							break;
						case 11:
						//Walls to the top, left and bottom
							tileSprite = palette.floorTiles [(int)FloorPalette.EndPieceLeft];
							break;
						case 12:
						//Walls to the right and bottom
							tileSprite = palette.floorTiles [(int)FloorPalette.BottomRight];
							break;
						case 13:
						//Walls to the right, left and bottom
							tileSprite = palette.floorTiles [(int)FloorPalette.EndPieceBottom];
							break;
						case 14:
						//Walls to the top, right and bottom
							tileSprite = palette.floorTiles [(int)FloorPalette.EndPieceRight];
							break;
						case 15:
						//Walls on all sides
							tileSprite = palette.floorTiles [(int)FloorPalette.FullyEnclosed];
							break;
						}	

						map [tempX, tempY].SetTile (tempX, tempY, false, false, false, tileSprite);
						map [tempX, tempY].SetVisibility (visibilityMap [tempX, tempY]);
					} else {
						if (tempX == 0) {
							if (tempY == 0) {
								//Bottom left
								if (collisionMap [tempX, tempY + 1] == 1
								    || collisionMap [tempX + 1, tempY] == 1
								    || collisionMap [tempX + 1, tempY + 1] == 1)
									wallMap [tempX, tempY] = 1;
							} else if (tempY == mapHeight - 1) {
								//Top left
								if (collisionMap [tempX + 1, tempY] == 1
								    || collisionMap [tempX, tempY - 1] == 1
								    || collisionMap [tempX + 1, tempY - 1] == 1)
									wallMap [tempX, tempY] = 1;
							} else {
								//Anywhere along the left edge
								if (collisionMap [tempX, tempY + 1] == 1
								    || collisionMap [tempX + 1, tempY] == 1
								    || collisionMap [tempX, tempY - 1] == 1
								    || collisionMap [tempX + 1, tempY - 1] == 1
								    || collisionMap [tempX + 1, tempY + 1] == 1)
									wallMap [tempX, tempY] = 1;
							}
						} else if (tempX == mapWidth - 1) {
							if (tempY == 0) {
								//Bottom right
								if (collisionMap [tempX - 1, tempY] == 1
								    || collisionMap [tempX, tempY + 1] == 1
								    || collisionMap [tempX - 1, tempY + 1] == 1)
									wallMap [tempX, tempY] = 1;
							} else if (tempY == mapHeight - 1) {
								//Top right
								if (collisionMap [tempX - 1, tempY] == 1
								    || collisionMap [tempX, tempY - 1] == 1
								    || collisionMap [tempX - 1, tempY - 1] == 1)
									wallMap [tempX, tempY] = 1;
							} else {
								//Anywhere along the right edge
								if (collisionMap [tempX - 1, tempY] == 1
								    || collisionMap [tempX, tempY + 1] == 1
								    || collisionMap [tempX, tempY - 1] == 1
								    || collisionMap [tempX - 1, tempY - 1] == 1
								    || collisionMap [tempX - 1, tempY + 1] == 1)
									wallMap [tempX, tempY] = 1;
							}
						} else if (tempY == 0) {
							//Anywhere along the bottom edge
							if (collisionMap [tempX - 1, tempY] == 1
							    || collisionMap [tempX, tempY + 1] == 1
							    || collisionMap [tempX + 1, tempY] == 1
							    || collisionMap [tempX - 1, tempY + 1] == 1
							    || collisionMap [tempX + 1, tempY + 1] == 1)
								wallMap [tempX, tempY] = 1;
						} else if (tempY == mapHeight - 1) {
							//Anywhere along the top edge
							if (collisionMap [tempX - 1, tempY] == 1
							    || collisionMap [tempX + 1, tempY] == 1
							    || collisionMap [tempX, tempY - 1] == 1
							    || collisionMap [tempX - 1, tempY - 1] == 1
							    || collisionMap [tempX + 1, tempY - 1] == 1)
								wallMap [tempX, tempY] = 1;
						} else {
							//Anywhere else!
							if (collisionMap [tempX - 1, tempY] == 1
							    || collisionMap [tempX, tempY + 1] == 1
							    || collisionMap [tempX + 1, tempY] == 1
							    || collisionMap [tempX, tempY - 1] == 1
							    || collisionMap [tempX - 1, tempY - 1] == 1
							    || collisionMap [tempX - 1, tempY + 1] == 1
							    || collisionMap [tempX + 1, tempY - 1] == 1
							    || collisionMap [tempX + 1, tempY + 1] == 1)
								wallMap [tempX, tempY] = 1;
						}
					}
				}
			}
		}

		//Place the wall tiles
		for (int tempX = 0; tempX < mapWidth; ++tempX) {
			for (int tempY = 0; tempY < mapHeight; ++tempY) {
				if (wallMap [tempX, tempY] == 1) {
					//using bits as flags, work out the index of the sprite for the tile
					int surroundingWalls = 0;

					if (tempX > 0)
					if (wallMap [tempX - 1, tempY] == 1)
						surroundingWalls = surroundingWalls | 1;

					if (tempY < mapWidth - 1)
					if (wallMap [tempX, tempY + 1] == 1)
						surroundingWalls = surroundingWalls | 2;

					if (tempX < mapWidth - 1)
					if (wallMap [tempX + 1, tempY] == 1)
						surroundingWalls = surroundingWalls | 4;

					if (tempY > 0)
					if (wallMap [tempX, tempY - 1] == 1)
						surroundingWalls = surroundingWalls | 8;

					switch (surroundingWalls) {
					case 0:
						//No surrounding walls
						tileSprite = palette.wallTiles [(int)WallPalette.SinglePiece];
						break;
					case 1:
						//Wall to the left
						tileSprite = palette.wallTiles [(int)WallPalette.Horizontal];
						break;
					case 2:
						//Wall to the top
						tileSprite = palette.wallTiles [(int)WallPalette.Vertical];
						break;
					case 3:
						//Walls to the left and top
						tileSprite = palette.wallTiles [(int)WallPalette.BottomRight];
						break;
					case 4:
						//Wall to the right
						tileSprite = palette.wallTiles [(int)WallPalette.Horizontal];
						break;
					case 5:
						//Walls to the left and right
						tileSprite = palette.wallTiles [(int)WallPalette.Horizontal];
						break;
					case 6:
						//Walls to the top and right
						tileSprite = palette.wallTiles [(int)WallPalette.BottomLeft];
						break;
					case 7:
						//Walls to the top, left and right
						tileSprite = palette.wallTiles [(int)WallPalette.BottomT];
						break;
					case 8:
						//Wall to the bottom
						tileSprite = palette.wallTiles [(int)WallPalette.Vertical];
						break;
					case 9:
						//Walls to the bottom and left
						tileSprite = palette.wallTiles [(int)WallPalette.TopRight];
						break;
					case 10:
						//Walls to the bottom and top
						tileSprite = palette.wallTiles [(int)WallPalette.Vertical];
						break;
					case 11:
						//Walls to the top, left and bottom
						tileSprite = palette.wallTiles [(int)WallPalette.RightT];
						break;
					case 12:
						//Walls to the right and bottom
						tileSprite = palette.wallTiles [(int)WallPalette.TopLeft];
						break;
					case 13:
						//Walls to the right, left and bottom
						tileSprite = palette.wallTiles [(int)WallPalette.TopT];
						break;
					case 14:
						//Walls to the top, right and bottom
						tileSprite = palette.wallTiles [(int)WallPalette.LeftT];
						break;
					case 15:
						//Walls on all sides
						tileSprite = palette.wallTiles [(int)WallPalette.FourWay];
						break;
					}
						
					map [tempX, tempY].SetTile (tempX, tempY, true, false, false, tileSprite);
					map [tempX, tempY].SetVisibility (visibilityMap [tempX, tempY]);
				}
			}
		}

	}

	/// <summary>
	/// Places the up and down stairs.
	/// Returns the x and y co-ordinates of the stairs (i.e. the entry point)
	/// </summary>
	int[] PlaceStairs (int[,] collisionMap)
	{
		List<int> stair1X = new List<int> ();
		List<int> stair2X = new List<int> ();
		int stair1Y = -1;
		int stair2Y = -1;
		int[] stairCoords = new int[4];

		//Starting from the edges of the screen, move inwards until we find
		// y-co-ords with suitable spots for the stairs. Then, place the
		// stairs at random points along these 'lines'
		bool stairSpot1Found = false;
		for (int tempX = 1; tempX < mapWidth / 2; ++tempX) {
			for (int tempY = 1; tempY < mapHeight - 1; ++tempY) {
				if (collisionMap [tempX, tempY] == 1) {
					if (collisionMap [tempX + 1, tempY] == 1
					    && collisionMap [tempX - 1, tempY] == 1
					    && collisionMap [tempX, tempY + 1] == 1
					    && collisionMap [tempX, tempY - 1] == 1) {
						stair1Y = tempY;
						stair1X.Add (tempX);
						stairSpot1Found = true;
					}
				}
			}
			if (stairSpot1Found)
				break;
		}

		bool stairSpot2Found = false;
		for (int tempX = mapWidth - 2; tempX > mapWidth / 2; --tempX) {
			for (int tempY = 1; tempY < mapHeight - 1; ++tempY) {
				if (collisionMap [tempX, tempY] == 1) {
					if (collisionMap [tempX + 1, tempY] == 1
					    && collisionMap [tempX - 1, tempY] == 1
					    && collisionMap [tempX, tempY + 1] == 1
					    && collisionMap [tempX, tempY - 1] == 1) {
						stair2Y = tempY;
						stair2X.Add (tempX);
						stairSpot2Found = true;
					}
				}
			}
			if (stairSpot2Found)
				break;
		}

		if (Random.value > 0.5f) {
			stairCoords [0] = stair1X [(int)((stair1X.Count - 1) * Random.value)];
			stairCoords [1] = stair1Y;
			stairCoords [2] = stair2X [(int)((stair2X.Count - 1) * Random.value)];
			stairCoords [3] = stair2Y;
		} else {
			stairCoords [0] = stair2X [(int)((stair2X.Count - 1) * Random.value)];
			stairCoords [1] = stair2Y;
			stairCoords [2] = stair1X [(int)((stair1X.Count - 1) * Random.value)];
			stairCoords [3] = stair1Y;
		}

		return stairCoords;
	}

	#endregion
}
