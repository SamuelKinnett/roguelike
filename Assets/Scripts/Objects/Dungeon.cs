using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class stores and handles a finite number of floors that form
/// the dungeon.
/// </summary>
public class Dungeon
{

	int numberOfFloors;
	List<DungeonFloor> floors;

	public Dungeon ()
	{
		floors = new List<DungeonFloor> ();
	}

	public void AddFloor (int width, int height, int[,] collisionMap, int[] stairLocations, bool bottomFloor)
	{
		floors.Add (new DungeonFloor (width, height, collisionMap, stairLocations, bottomFloor));
		++numberOfFloors;
	}

	public DungeonFloor GetFloor (int level)
	{
		return floors [level];
	}

	public void SetVisibilityMap (TileVisibility[,] visibilityMap, int level)
	{
		floors [level].SetVisibilityMap (visibilityMap);
	}

}
