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
        NPCManager.stats tempStats = new NPCManager.stats();
        tempStats.agility = 4;
        tempStats.armour = 2;
        tempStats.hp = 10;
        tempStats.strength = 4;
        tempStats.wisdom = 0;
		

		for (int curEnemy = 0; curEnemy < numberOfEnemies; curEnemy++) {
			enemies.Add (new NPCManager ());
			enemies [curEnemy].obj_MapManager = obj_MapManager;
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
				playerKilled = (EnemyAttackPlayer (curNPC.GetAllEnemyStats()));
			} else {
				curNPC.Move (playerX, playerY);
			}
		}

		return playerKilled;
	}

	/// <summary>
	/// If possile, move the player in the specified direction. If not, return false.
	/// If an enemy is in the way, attack them.
	/// </summary>
	/// <returns><c>true</c>, if player was moved, <c>false</c> otherwise.</returns>
	/// <param name="direction">Direction.</param>
	public bool MovePlayer (directions direction)
	{
		TileInfo targetTile;
		int[,] tempEnemyMap = new int[mapManager.mapWidth, mapManager.mapHeight];
		for (int curEnemy = 0; curEnemy < enemies.Count; ++curEnemy) {
			tempEnemyMap[enemies[curEnemy].GetPosition()[0], enemies[curEnemy].GetPosition()[1]] = curEnemy + 1;
		}

		switch (direction) {
		case directions.north:
			targetTile = mapManager.GetTile (playerX, playerY + 1);
			if (!targetTile.solid && tempEnemyMap [playerX, playerY + 1] == 0) {
				//The player can move
				++playerY;
				playerController.SetPosition (playerX, playerY);
				return true;
			} else if (tempEnemyMap [playerX, playerY + 1] > 0) {
				//Attack the enemy. If the player kills them, move into the space.
				if (PlayerAttackEnemy (playerController.GetAllPlayerStats (), tempEnemyMap [playerX, playerY + 1] - 1)) {
					++playerY;
					//playerController.SetPosition (playerX, playerY);
					return true;
				}
			}
			break;
		case directions.east:
			targetTile = mapManager.GetTile (playerX + 1, playerY);
			if (!targetTile.solid && tempEnemyMap [playerX + 1, playerY] == 0) {
				++playerX;
				playerController.SetPosition (playerX, playerY);
				return true;
			} else if (tempEnemyMap [playerX + 1, playerY] > 0) {
				//Attack the enemy. If the player kills them, move into the space.
				if (PlayerAttackEnemy (playerController.GetAllPlayerStats (), tempEnemyMap [playerX + 1, playerY] - 1)) {
					++playerX;
					//playerController.SetPosition (playerX, playerY);
					return true;
				}
			}
			break;
		case directions.south:
			targetTile = mapManager.GetTile (playerX, playerY - 1);
			if (!targetTile.solid && tempEnemyMap [playerX, playerY - 1] == 0) {
				--playerY;
				playerController.SetPosition (playerX, playerY);
				return true;
			} else if (tempEnemyMap [playerX, playerY - 1] > 0) {
				//Attack the enemy. If the player kills them, move into the space.
				if (PlayerAttackEnemy (playerController.GetAllPlayerStats (), tempEnemyMap [playerX, playerY - 1] - 1)) {
					--playerY;
					//playerController.SetPosition (playerX, playerY);
					return true;
				}
			}
			break;
		case directions.west:
			targetTile = mapManager.GetTile (playerX - 1, playerY);
			if (!targetTile.solid && tempEnemyMap [playerX - 1, playerY] == 0) {
				--playerX;
				playerController.SetPosition (playerX, playerY);
				return true;
			} else if (tempEnemyMap [playerX - 1, playerY] > 0) {
				//Attack the enemy. If the player kills them, move into the space.
				if (PlayerAttackEnemy (playerController.GetAllPlayerStats (), tempEnemyMap [playerX - 1, playerY] - 1)) {
					--playerX;
					//playerController.SetPosition (playerX, playerY);
					return true;
				}
				return false;
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
	public bool EnemyAttackPlayer (NPCManager.stats enemyStats)
	{
		return (playerController.PlayerHit (enemyStats));
	}

	public bool PlayerAttackEnemy (PlayerController.stats playerStats, int enemyIndex)
	{
		if (enemies [enemyIndex].NPCHit (playerStats)) {
			enemies.RemoveAt (enemyIndex);
			return true;
		}
		return false;
	}

	public void updateEntityCollisionKnowledge() {
		foreach (NPCManager currentEnemy in enemies) {
			bool[] adjacentEntities = new bool[4];
			adjacentEntities [0] = CheckCollision (currentEnemy.GetPosition () [0] - 1, currentEnemy.GetPosition () [1]);
			adjacentEntities [1] = CheckCollision (currentEnemy.GetPosition () [0], currentEnemy.GetPosition () [1] + 1);
			adjacentEntities [2] = CheckCollision (currentEnemy.GetPosition () [0] + 1, currentEnemy.GetPosition () [1]);
			adjacentEntities [3] = CheckCollision (currentEnemy.GetPosition () [0], currentEnemy.GetPosition () [1] - 1);
			currentEnemy.UpdateAdjacentEntities (adjacentEntities);
		}
	}

	public bool CheckCollision (int x, int y) {
		foreach (NPCManager currentEnemy in enemies) {
			if (currentEnemy.CheckCollision(x, y) || (x == playerX && y == playerY))
				return true;
		}
		return false;
	}
}
