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

	public Dungeon() {
		floors = new List<DungeonFloor> ();
	}

	public void AddFloor (int width, int height, int[,] collisionMap, int[] stairLocations, bool bottomFloor)
	{
		Debug.Log ("Added a new floor, my chum");
		floors.Add (new DungeonFloor (width, height, collisionMap, stairLocations, bottomFloor));
		Debug.Log (floors.Count);
		++numberOfFloors;
	}

	public DungeonFloor GetFloor(int level) {
		Debug.Log (floors.Count);
		return floors [level];
	}

	public void SaveVisibilityMap(TileVisibility[,] visibilityMap, int level) {
		floors [level].SaveVisibilityMap (visibilityMap);
	}

	public TileVisibility[,] GetVisbilityMap(int level) {
		return floors [level].GetVisibilityMap ();
	}

}
