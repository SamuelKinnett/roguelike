using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject obj_MapManager;
	public GameObject obj_EntityManager;
    public int        direction;
    

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
        bool input = true;
        //GetInput
        while (input == true)
        {
            if (Input.GetKey("up")) { direction = 0; }  //change keys later so user can bind
            else if (Input.GetKey("right")) { direction = 1; }
            else if (Input.GetKey("down")) { direction = 2; }
            else if (Input.GetKey("left")) { direction = 3; }

            //check the player can move
            if (EntityManager.MovePlayer(direction) == true)
            {
                input = false;
                if (EntityManager.OnStairs() == true)
                {
                    MapManager.GenerateMap();
                }
            }
        }
        

        //		
        //	UpdateEntities();
    }
}
