using UnityEngine;
using System.Collections;

public class NPCManager : MonoBehaviour {

    public GameObject obj_MapManager;
    public GameObject obj_EntityManager;

    public SpriteRenderer spriteRenderer;
    private float worldWidth, worldHeight;
    private EntityManager entityManager;
    private MapManager mapManager;

    int visualRadius;
    int x;
    int y;


    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        worldWidth = spriteRenderer.bounds.size.x;
        worldHeight = spriteRenderer.bounds.size.y;

        mapManager = obj_MapManager.GetComponent<MapManager> ();
		entityManager = obj_EntityManager.GetComponent<EntityManager> ();

        int[] randomPosition = new int[2] = mapManager.GetRandomPosition();
        x = randomPosition[0];
        y = randomPosition[1];
        visualRadius = 5;
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    public void MoveNPC()
    {
        int playerX = entityManager.GetPlayerPosition()[0];
        int playerY = entityManager.GetPlayerPosition()[1];
        //visual detection
        if (playerX >= ( x- visualRadius) && playerX <= (x + visualRadius) && playerY >= (y - visualRadius) && playerY <= (y + visualRadius))
        {
            Movement(playerX, playerY);
        }
    }

    public void Movement(int playerX, int playerY)
    {

    }
}
