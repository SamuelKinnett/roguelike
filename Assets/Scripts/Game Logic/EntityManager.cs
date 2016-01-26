using UnityEngine;
using System.Collections;

public class EntityManager : MonoBehaviour {

	public GameObject obj_MapManager;
	public GameObject obj_Player;

	private MapManager mapManager;
	private PlayerController playerController;

	// Use this for initialization
	void Start () {
		mapManager = obj_MapManager.GetComponent<MapManager> ();
		playerController = obj_Player.GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Place the player at the specicified position and generate enemies
	void Initialise(int playerX, int playerY) {

	}

	/// <summary>
	/// Updates the NPCs in the game world. All update logic goes here
	/// </summary>
	void UpdateNPC() {
		//Move NPCS
		//Handle combat
	}

	/// <summary>
	/// Spawns the specified number of a selection of enemies
	/// </summary>
	/// <param name="difficulty">Difficulty.</param>
	/// <param name="count">Count.</param>
	void SpawnEnemies(float difficulty, int count) {

	}

	/// <summary>
	/// If possile, move the player in the specified direction. If not, return false.
	/// </summary>
	/// <returns><c>true</c>, if player was moved, <c>false</c> otherwise.</returns>
	/// <param name="direction">Direction.</param>
	bool MovePlayer(int direction) {

	}

	/// <summary>
	/// Returns true if the player is on the stairs (descending)
	/// </summary>
	bool OnStairs() {

	}
}
