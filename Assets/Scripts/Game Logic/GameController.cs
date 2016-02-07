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
        //Game logic is here
        bool input = true;
        int direction = -1;
        //GetInput
        while (input == true)
        {
            //TODO: Change so user can bind keys
            if      (Input.GetKey("up"))    { direction = 0; }  
            else if (Input.GetKey("right")) { direction = 1; }
            else if (Input.GetKey("down"))  { direction = 2; }
            else if (Input.GetKey("left"))  { direction = 3; }

            //ensure that only valid keys have been pushed
            if (direction != -1)
            {
                //check the player can move into the position on the map
                if (EntityManager.MovePlayer(direction) == true)
                {
                    //player has moved - no more user input wanted
                    input = false;
                    //check if player is on stairs
                    if (EntityManager.OnStairs() == true)
                    {
                        //player goes up or down stairs
                        MapManager.GenerateMap();
                    }
                }
            }
        }
        

        //		
        //	UpdateEntities();
    }
}
