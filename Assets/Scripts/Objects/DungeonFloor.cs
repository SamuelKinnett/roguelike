using UnityEngine;
using System.Collections;

/// <summary>
/// This class stores information about a particular floor of a dungeon
/// </summary>
public class DungeonFloor
{
	int width;
	int height;
	int[,] collisionMap;
	int[] stairLocations;
	bool bottomFloor;
	TileVisibility[,] visibilityMap;

	public DungeonFloor (int[,] collisionMap, int[] stairLocations, bool bottomFloor)
	{
		this.collisionMap = collisionMap;
		this.stairLocations = stairLocations;
		this.bottomFloor = bottomFloor;
	}

	public int[,] GetCollisionMap ()
	{
		return collisionMap;
	}

	public int[] GetStairPositions ()
	{
		return stairLocations;
	}
}
