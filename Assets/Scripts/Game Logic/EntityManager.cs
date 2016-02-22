using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	List<NPCManager> enemies;

	int playerX;
	int playerY;

	//TESTING
	public Sprite testEnemySprite;

	// Use this for initialization
	void Start ()
	{
		mapManager = obj_MapManager.GetComponent<MapManager> ();
		playerController = obj_Player.GetComponent<PlayerController> ();
	}

	// Place the player at the specicified position and generate enemies
	public void Initialise (int playerX, int playerY)
	{
		this.playerX = playerX;
		this.playerY = playerY;
		playerController.SetPosition (playerX, playerY);

		//Generate enemies
		if (enemies != null) {
			foreach (NPCManager curNPC in enemies) {
				curNPC.DestroyNPC ();
			}
		}

		int numberOfEnemies = (int)(5 * Random.value + 1);	//Generate between 1 and 6 (both inclusive) enemies
		enemies = new List<NPCManager> ();

		//Temporary placeholder stats
		int[] tempStats = new int[6];
		tempStats [(int)stats.agility] = 4;
		tempStats [(int)stats.armour] = 2;
		tempStats [(int)stats.attack] = 4;
		tempStats [(int)stats.hp] = 10;
		tempStats [(int)stats.magicArmour] = 0;
		tempStats [(int)stats.magicAttack] = 0;

		for (int curEnemy = 0; curEnemy < numberOfEnemies; curEnemy++) {
			enemies.Add (new NPCManager ());
			enemies [curEnemy].CreateNPC (tempStats, testEnemySprite);
		}

	}

	/// <summary>
	/// Updates the NPCs in the game world. All update logic goes here
	/// If the player has been killed by the NPCS, return true.
	/// </summary>
	public bool UpdateNPCs ()
	{
		bool playerKilled = false;
		foreach (NPCManager curNPC in enemies) {
			if (curNPC.CanAttack (playerX, playerY)) {
				playerKilled = (EnemyAttackPlayer (curNPC.GetStats ()));
			} else {
				curNPC.Move (playerX, playerY);
			}
		}

		return playerKilled;
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

	/// <summary>
	/// Handles an attack on the player by an enemy. If the player is killed, return true.
	/// </summary>
	/// <returns><c>true</c>, if attack player was enemyed, <c>false</c> otherwise.</returns>
	/// <param name="enemyStats">Enemy stats.</param>
	bool EnemyAttackPlayer (int[] enemyStats)
	{
		return (playerController.PlayerHit (enemyStats));
	}
}
