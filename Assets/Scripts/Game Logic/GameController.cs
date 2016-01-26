using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject obj_MapManager;
	public GameObject obj_EntityManager;

	private MapManager mapManager;
	private EntityManager entityManager;

	// Use this for initialization
	void Start () {
		mapManager = obj_MapManager.GetComponent<MapManager> ();
		entityManager = obj_EntityManager.GetComponent<EntityManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Game logic goes here

		//GetInput
		//If the player can move
		//	MovePlayer();
		//	Check if the player is on stairs();
		//		GenerateMap();
		//		
		//	UpdateEntities();
	}
}
