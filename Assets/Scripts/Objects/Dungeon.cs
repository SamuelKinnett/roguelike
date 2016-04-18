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
	int[] enemyLevelRange;
	string dungeonName;
	string mapPalette;

	FileManager fileManager;
	List<DungeonFloor> floors;

	public Dungeon ()
	{
		floors = new List<DungeonFloor> ();
		fileManager = GameObject.Find ("FileManager").GetComponent<FileManager>();

		GenerateName ();
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

	void GenerateName() {
		int roll = (int)Mathf.Round (5 * Random.value);
		string owner;

		switch (roll) {

		case 0:
			//prefix and first
			//e.g. "The whispering meadow"

			dungeonName = "The " + fileManager.GetDungeonPrefix() + " " + fileManager.GetDungeonFirstName() + ".";

			break;

		case 1:
			//first and suffix
			//e.g. "The caves of sorrow"

			dungeonName = "The " + fileManager.GetDungeonFirstName () + " of " + fileManager.GetDungeonSuffix() + ".";

			break;

		case 2:
			//first and second
			//e.g. "The Castle of Jim"

			dungeonName = "The " + fileManager.GetDungeonFirstName () + " of " + fileManager.GetDungeonSecondName() + ".";

			break;

		case 3:
			//second and first
			//e.g. "Sen's fortress"

			owner = fileManager.GetDungeonSecondName ();
			if (owner.ToCharArray () [owner.ToCharArray ().Length - 1] == 's')
				owner += "'";
			else
				owner += "'s";

			dungeonName = owner + " " + fileManager.GetDungeonFirstName () + ".";

			break;

		case 4:
			//second, first and suffix
			//e.g. "Sen's fortress of clanging"

			owner = fileManager.GetDungeonSecondName ();
			if (owner.ToCharArray () [owner.ToCharArray ().Length - 1] == 's')
				owner += "'";
			else
				owner += "'s";

			dungeonName = owner + " " + fileManager.GetDungeonFirstName () + " of " + fileManager.GetDungeonSuffix() + ".";

			break;

		case 5:
			//second, prefix, first and suffix
			//e.g. "Sen's massive fortress of smashing"

			owner = fileManager.GetDungeonSecondName ();
			if (owner.ToCharArray () [owner.ToCharArray ().Length - 1] == 's')
				owner += "'";
			else
				owner += "'s";

			dungeonName = owner + " " + fileManager.GetDungeonPrefix() + " "
				+ fileManager.GetDungeonFirstName () + " of " + fileManager.GetDungeonSuffix() + ".";
			break;

		}

		Debug.Log (dungeonName);
	}

}
