using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	public GameObject obj_MapManager;
	public GameObject obj_EntityManager;
	public string paletteName;

	private MapManager mapManager;
	private EntityManager entityManager;

	bool newMapNeeded;

	//flags used for the gameloop
	bool playerMoved;
	bool fogUpdated;
	bool entitiesUpdated;
	bool keyReleased;
	string key;
	bool resetLoop;

	// Use this for initialization
	void Start ()
	{
		mapManager = obj_MapManager.GetComponent<MapManager> ();
		entityManager = obj_EntityManager.GetComponent<EntityManager> ();
		newMapNeeded = true;

		playerMoved = false;
		keyReleased = false;
		entitiesUpdated = false;
		fogUpdated = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Game logic goes here

		if (newMapNeeded) {
			int[] playerPos = mapManager.GenerateMap ();
			entityManager.Initialise (playerPos [0], playerPos [1]);
			mapManager.RecalculateFogOfWar (playerPos [0], playerPos [1]);
			newMapNeeded = false;
			playerMoved = false;
			keyReleased = false;
			entitiesUpdated = false;
			fogUpdated = false;
		}
			

		//GetInput
		if (!playerMoved) {
			
			directions direction = directions.north;
			bool moveKeyPressed = false;

			//TODO: Change so user can bind keys
			if (Input.GetKey ("up")) {
				direction = directions.north;
				key = "up";
				moveKeyPressed = true;
			} else if (Input.GetKey ("right")) {
				direction = directions.east;
				key = "right";
				moveKeyPressed = true;
			} else if (Input.GetKey ("down")) {
				direction = directions.south;
				key = "down";
				moveKeyPressed = true;
			} else if (Input.GetKey ("left")) {
				direction = directions.west;
				key = "left";
				moveKeyPressed = true;
			}

			//ensure that only valid keys have been pushed
			if (moveKeyPressed) {
				//check the player can move into the position on the map
				if (entityManager.MovePlayer (direction) == true) {
					//player has moved - no more user input wanted
					playerMoved = true;
					//check if player is on stairs
					if (entityManager.OnDownStairs ()) {
						//make the player go to a new floor
						int[] playerStartPosition = mapManager.GenerateMap ();
						entityManager.Initialise (playerStartPosition [0],
							playerStartPosition [1]);
					}
				}
			}
		} else if (!fogUpdated) {
			int[] playerPosition = entityManager.GetPlayerPosition ();
			mapManager.RecalculateFogOfWar (playerPosition [0], playerPosition [1]);
			fogUpdated = true;
		} else if (!keyReleased) {
			//Prevents the issue of the user holding down the movement key.
			//	in future, a better implementation should be used to allow for
			//	holding the key but preventing difficult movement.
			if (Input.GetKeyUp (key))
				keyReleased = true;
		} else if (!entitiesUpdated) {
			//	UpdateEntities();
			entityManager.UpdateEntityPositions ();
			entitiesUpdated = true;
		} else {
			//Reset the flags
			playerMoved = false;
			keyReleased = false;
			entitiesUpdated = false;
			fogUpdated = false;
		}
	}
}
