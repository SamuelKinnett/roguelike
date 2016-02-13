using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	public GameObject obj_MapManager;
	public GameObject obj_EntityManager;
	public string paletteName;

	private MapManager mapManager;
	private EntityManager entityManager;
	private GameParams gameParameters;

	bool newMapNeeded;
	bool paused;

	//flags used for the gameloop
	bool playerMoved;
	bool fogUpdated;
	bool entitiesUpdated;
	bool pauseFinished;
	string key;
	bool resetLoop;
	float pauseCounter;

	// Use this for initialization
	void Start ()
	{
		//Load the game parameters
		GameObject obj_GameParams = GameObject.Find("GameParameters");
		gameParameters = obj_GameParams.GetComponent<GameParams> ();

		mapManager = obj_MapManager.GetComponent<MapManager> ();
		entityManager = obj_EntityManager.GetComponent<EntityManager> ();
		newMapNeeded = true;

		playerMoved = false;
		pauseFinished = false;
		entitiesUpdated = false;
		fogUpdated = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Game logic goes here

		if (newMapNeeded) {
			int[] playerPos = mapManager.GenerateMap (gameParameters.mapWidth,
				gameParameters.mapHeight,
				gameParameters.paletteName);
			entityManager.Initialise (playerPos [0], playerPos [1]);
			mapManager.RecalculateFogOfWar (playerPos [0], playerPos [1]);
			newMapNeeded = false;
			playerMoved = false;
			pauseFinished = false;
			entitiesUpdated = false;
			fogUpdated = false;
		}
			
		if (!paused) {
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
							int[] playerStartPosition = mapManager.GenerateMap (gameParameters.mapWidth,
								gameParameters.mapHeight,
								gameParameters.paletteName);
							entityManager.Initialise (playerStartPosition [0],
								playerStartPosition [1]);
						}
					}
				}
			} else if (!fogUpdated) {
				int[] playerPosition = entityManager.GetPlayerPosition ();
				mapManager.RecalculateFogOfWar (playerPosition [0], playerPosition [1]);
				fogUpdated = true;
			} else if (!pauseFinished) {
				//Prevents the issue of the player character moving too quickly by adding
				// a small pause. This pause is broken if the player releases the key.
				pauseCounter += Time.deltaTime;
				if (Input.GetKeyUp (key) || pauseCounter > 0.1f)
					pauseFinished = true;
			} else if (!entitiesUpdated) {
				//	UpdateEntities();
				entityManager.UpdateEntityPositions ();
				entitiesUpdated = true;
			} else {
				//Reset the flags
				playerMoved = false;
				pauseFinished = false;
				entitiesUpdated = false;
				fogUpdated = false;
				pauseCounter = 0;
			}
		}
	}

	/// <summary>
	/// Used to pause the game loop, preventing input
	/// </summary>
	public void Pause ()
	{
		paused = true;
	}

	/// <summary>
	/// Used to resume the game loop after a pause
	/// </summary>
	public void Resume ()
	{
		paused = false;
	}
}
