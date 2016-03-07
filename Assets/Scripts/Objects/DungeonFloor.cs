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

	public DungeonFloor (int width, int height, int[,] collisionMap, int[] stairLocations, bool bottomFloor)
	{
		this.width = width;
		this.height = height;
		this.collisionMap = collisionMap;
		this.stairLocations = stairLocations;
		this.bottomFloor = bottomFloor;
		visibilityMap = new TileVisibility[width, height];
	}

	public int[,] GetCollisionMap ()
	{
		return collisionMap;
	}

	public int[] GetStairPositions ()
	{
		return stairLocations;
	}

	public void SetVisibilityMap (TileVisibility[,] visibilityMap)
	{
		this.visibilityMap = visibilityMap;
	}

	public TileVisibility[,] GetVisibilityMap ()
	{
		return visibilityMap;
	}

}
