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
			
			float splitAmount;

			children = new MapNode[2];

			//The split percentage should be between 40% and 60% of the parent node
			splitAmount = 0.2f * rand.NextDouble + 0.4f;

			//alternate between horizontal and vertical splits on each layer
			if (depth % 2) {
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
				JoinRooms (children [0], children [1]);
			}

			return collisionMap;
		}

		//Connects two rooms or regions with a corridor
		void JoinRooms (MapNode room1, MapNode room2)
		{
			if (room1.depth % 2) {
				//The rooms have been split horizontally, so we need a vertical line.
				
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


		emptyListUpdateNeeded = true;
	}

	#endregion
}
