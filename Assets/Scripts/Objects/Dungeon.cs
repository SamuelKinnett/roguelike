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

	public void AddFloor (int[,] collisionMap, int[] stairLocations, bool bottomFloor)
	{
		floors.Add (new DungeonFloor (collisionMap, stairLocations, bottomFloor));
		++numberOfFloors;
	}
}
