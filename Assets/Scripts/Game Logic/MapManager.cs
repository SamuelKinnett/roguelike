using UnityEngine;
using System.Collections;

public struct TileInfo {
	public int x;
	public int y;
	public bool solid;
	public bool stairsUp;
	public bool stairsDown;
}

public class MapManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Returns a TileInfo structure for the tile at the specified position
	/// </summary>
	TileInfo GetTile(int x, int y) {

	}

	/// <summary>
	/// Returns the x and y co-ordinate of a pseudo-random non-collidable tile 
	/// </summary>
	int[] GetRandomPosition() {

	}

	/// <summary>
	/// Called to pseudo-randomly generate a map.
	/// Returns the co-ordinates of the starting stairs
	/// </summary>
	int[] GenerateMap() {

	}
}
