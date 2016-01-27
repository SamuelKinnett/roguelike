using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{

	MapTile[,] map;
	//This 2D array contains all of the map tiles
	List<MapTile> emptyTiles;
	//A list of all non-collideable (i.e. empty) tiles
	int mapWidth, mapHeight;
	bool emptyListUpdateNeeded;
	//Do we need to populate the list of empty tiles?

	// Use this for initialization
	void Start ()
	{
		emptyListUpdateNeeded = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
	public int[] GetRandomPosition ()
	{
		//Create and instantiate a new random number generator
		System.Random rand = new System.Random ();

		if (emptyListUpdateNeeded)
			UpdateEmptyTileList ();

		//Return the index of a random empty tile
		int returnTileIndex = (int)((emptyTiles.Count - 1) * rand.NextDouble ()); 

		int[] returnArray = new int[2];
		returnArray [0] = emptyTiles [returnTileIndex].GetInfo ().x;
		returnArray [1] = emptyTiles [returnTileIndex].GetInfo ().y;

		return returnArray;
	}

	/// <summary>
	/// Populates a list of all non-collideable tiles.
	/// </summary>
	void UpdateEmptyTileList ()
	{

		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				if (!map [x, y].GetInfo ().solid)
					emptyTiles.Add (map [x, y]);
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
		MapNode[] children;
		System.Random rand;

		public MapNode (int x, int y, int width, int height, int depth)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.depth = depth;

			rand = new System.Random ();
		}

		/// <summary>
		/// Recursively split the map down into chunks, stored as a tree
		/// </summary>
		/// <param name="desiredDepth">Desired depth.</param>
		public void Split (int desiredDepth)
		{

			if (depth == desiredDepth)
				return;
			
			double splitAmount;

			children = new MapNode[2];

			//The split percentage should be between 40% and 60% of the parent node
			splitAmount = 0.2f * rand.NextDouble () + 0.4f;

			//alternate between horizontal and vertical splits on each layer
			if (depth % 2 == 1) {
				//split horizontally
				int splitLocation = (int)(height * splitAmount);
				children [0] = new MapNode (x, y, width, height - splitLocation, depth + 1);
				children [0].Split (desiredDepth);
				children [1] = new MapNode (x, y + splitLocation, width, splitLocation, depth + 1);
				children [1].Split (desiredDepth);
			} else {
				//split vertically
				int splitLocation = (int)(width * splitAmount);
				children [0] = new MapNode (x, y, width - splitLocation, height, depth + 1);
				children [0].Split (desiredDepth);
				children [1] = new MapNode (x + splitLocation, y, splitLocation, height, depth + 1);
				children [1].Split (desiredDepth);
			}
		}

		/// <summary>
		/// Places rooms randomly at the specified depth and connects them with
		/// corridors.
		/// Returns a 2D array containing the collision map of the dungeon
		/// </summary>
		public int[,] PlaceRooms (int desiredDepth, int[,] collisionMap)
		{
			if (depth == desiredDepth) {

				System.Random rand = new System.Random ();

				//Decide on the position of the room and then the dimensions
				//Place the origin of the room somewhere within the lower left corner
				//of the region.
				int roomX = (int)((width / 2) * rand.NextDouble ());
				int roomY = (int)((height / 2) * rand.NextDouble ());
				int roomWidth = (int)(((width - roomX) / 4) * rand.NextDouble () + ((width - roomX) / 2));
				int roomHeight = (int)(((height - roomY) / 4) * rand.NextDouble () + ((height - roomY) / 2));

				for (int tempX = roomX; tempX < roomWidth; ++tempX) {
					for (int tempY = roomY; tempY < roomHeight; ++tempY) {
						collisionMap [tempX, tempY] = 1;	//Make this area not solid.
					}
				}
			} else {
				//Call the function recursively
				collisionMap = children [0].PlaceRooms (desiredDepth, collisionMap);
				collisionMap = children [1].PlaceRooms (desiredDepth, collisionMap);
				//After this, link the two with a corridor.
				collisionMap = JoinRooms (children [0], children [1], collisionMap);
			}

			return collisionMap;
		}

		/// <summary>
		/// Connects two rooms or regions with a corridor
		/// It's important to note that die to the way splitting is handled, the first
		/// room will always be below or to the left of the second room.
		/// </summary>
		int[,] JoinRooms (MapNode room1, MapNode room2, int[,] collisionMap)
		{
			if (room1.depth % 2 == 1) {
				//The rooms have been split horizontally, so we need a vertical line.

				//Firstly, find a line halfway between the edges of the two rooms
				//well, near-as-damnit anyway.

				int topRoomY, bottomRoomY;
				bool spaceFound = false;

				//We'll need to start by finding the y value of the two edges. Look from
				//top to bottom so that we know the first space encountered will be the
				//"highest" empty point in the lower area.
				for (int tempX = room1.x; tempX < room1.width; ++tempX) {
					for (int tempY = room1.height - 1; tempY >= room1.y; --tempY) {
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

				//Now, calculate the "lowest" y value of the top area
				spaceFound = false;
				for (int tempX = room2.x; tempX < room2.width; ++tempX) {
					for (int tempY = room2.y; tempY < room2.height; ++tempY) {
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

				//Next, decide on 

			} else {
				//The rooms have been split vertically, so we need a horizontal line.

			}
		}
	}

	/// <summary>
	/// Called to pseudo-randomly generate a map.
	/// Returns the co-ordinates of the starting stairs
	/// </summary>
	public int[] GenerateMap ()
	{
		int[,] collisionMap = new int[mapWidth, mapHeight];

		MapNode mapGenerator = new MapNode (0, 0, mapWidth, mapHeight, 0);
		mapGenerator.Split (3);	//This will split the map 3 times

		//Now, generate the collision map
		mapGenerator.PlaceRooms (3, collisionMap);

		//TODO: add code here to place doors and subsequently:
		//	-distinguish between rooms and corridors
		//	-set the maptiles to represent the map
		//	-place stairs

		//for now, return a placeholder
		int[] stairPosition = new int[2];
		stairPosition [0] = 0;
		stairPosition [1] = 0;

		emptyListUpdateNeeded = true;	//Now that we've generated the map, it'll need an update to the empty list

		return stairPosition;
	}

	#endregion
}
