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
		this.collisionMap = collisionMap;
		this.stairLocations = stairLocations;
		this.bottomFloor = bottomFloor;
		visibilityMap = new TileVisibility[width, height];
		this.width = width;
		this.height = height;
	}

	public int[,] GetCollisionMap ()
	{
		return collisionMap;
	}

	public int[] GetStairPositions ()
	{
		return stairLocations;
	}

	public void SaveVisibilityMap(TileVisibility[,] tileVisibility) {
		this.visibilityMap = tileVisibility;
	}

	public TileVisibility[,] GetVisibilityMap() {
		return visibilityMap;
	}

}
