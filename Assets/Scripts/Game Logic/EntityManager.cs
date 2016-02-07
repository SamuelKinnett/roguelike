using UnityEngine;
using System.Collections;

public enum directions
{
	north,
	east,
	south,
	west
}


public class EntityManager : MonoBehaviour
{


	public GameObject obj_MapManager;
	public GameObject obj_Player;

	private MapManager mapManager;
	private PlayerController playerController;

	int playerX;
	int playerY;

	// Use this for initialization
	void Start ()
	{
		mapManager = obj_MapManager.GetComponent<MapManager> ();
		playerController = obj_Player.GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	// Place the player at the specicified position and generate enemies
	public void Initialise (int playerX, int playerY)
	{
		this.playerX = playerX;
		this.playerY = playerY;
		playerController.SetPosition (playerX, playerY);
	}

	/// <summary>
	/// Updates the NPCs in the game world. All update logic goes here
	/// </summary>
	void UpdateNPC ()
	{
		//Move NPCS
		//Handle combat
	}

	/// <summary>
	/// Spawns the specified number of a selection of enemies
	/// </summary>
	/// <param name="difficulty">Difficulty.</param>
	/// <param name="count">Count.</param>
	void SpawnEnemies (float difficulty, int count)
	{

	}

	/// <summary>
	/// If possile, move the player in the specified direction. If not, return false.
	/// </summary>
	/// <returns><c>true</c>, if player was moved, <c>false</c> otherwise.</returns>
	/// <param name="direction">Direction.</param>
	public bool MovePlayer (directions direction)
	{
		TileInfo targetTile;

		switch (direction) {
		case directions.north:
			targetTile = mapManager.GetTile (playerX, playerY + 1);
			if (!targetTile.solid) {
				//The player can move
				++playerY;
				playerController.SetPosition (playerX, playerY);
				return true;
			}
			break;
		case directions.east:
			targetTile = mapManager.GetTile (playerX + 1, playerY);
			if (!targetTile.solid) {
				++playerX;
				playerController.SetPosition (playerX, playerY);
				return true;
			}
			break;
		case directions.south:
			targetTile = mapManager.GetTile (playerX, playerY - 1);
			if (!targetTile.solid) {
				--playerY;
				playerController.SetPosition (playerX, playerY);
				return true;
			}
			break;
		case directions.west:
			targetTile = mapManager.GetTile (playerX - 1, playerY);
			if (!targetTile.solid) {
				--playerX;
				playerController.SetPosition (playerX, playerY);
				return true;
			}
			break;
                
		}

		return false;
	}

	/// <summary>
	/// Returns true if the player is on the stairs (descending)
	/// </summary>
	public bool OnDownStairs ()
	{
		TileInfo targetTile;
		targetTile = mapManager.GetTile (playerX, playerY);
		if (targetTile.stairsDown) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns true if the player is on the stairs (acsending)
	/// </summary>
	public bool OnUpStairs ()
	{
		TileInfo targetTile;
		targetTile = mapManager.GetTile (playerX, playerY);
		if (targetTile.stairsUp) {
			return true;
		} else {
			return false;
		}
	}

	public void UpdateEntityPositions ()
	{
		//playerController.SetPosition (playerX, playerY);
	}

	public int[] GetPlayerPosition ()
	{
		int[] position = new int[2];
		position [0] = playerX;
		position [1] = playerY;

		return position;
	}

}
