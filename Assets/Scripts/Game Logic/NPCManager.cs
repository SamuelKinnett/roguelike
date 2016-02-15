using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour {

    public GameObject obj_MapManager;
    public GameObject obj_EntityManager;
    public GameObject obj_Node;
    
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

        int[] randomPosition = new int[2];
        randomPosition = mapManager.GetRandomPosition();
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
            Movement(playerX, playerY);//returns int[] of movment position
        }
    }

    public int[] Movement(int playerX, int playerY)
    {
        //A* psudocode -> http://web.mit.edu/eranki/www/tutorials/search/
        Node npc = obj_Node.GetComponent<Node>();
        npc.Initilise(x, y, 0, 0, 0);
        Node q = npc;

        List<Node> openL = new List<Node>();
        List<Node> closeL = new List<Node>();
        
        openL.Add(npc);

        while (openL.Count < 0)
        {
            float qf = 100;
            int i = 0;
            foreach (Node n in openL)
            {
                if (qf > n.GetF())
                {
                    qf = n.GetF();
                    q = n;
                    openL.RemoveAt(i);  
                }
                i++;
            }
            if (q.GetX() == playerX && q.GetY() == playerY)
            {
                return reconstruct_path(q);
            }
            //up
            
            if (!mapManager.GetTile(q.GetX(), (q.GetY() + 1)).solid)
            {
                openL = successorNode(q, q.GetX(), (q.GetY() + 1), playerX, playerY,openL,closeL);
            }
            closeL.Add(q);
            //down
            if (!mapManager.GetTile(q.GetX(), (q.GetY() - 1)).solid)
            {
                openL = successorNode(q, q.GetX(), (q.GetY() - 1), playerX, playerY, openL, closeL);
            }
            closeL.Add(q);
            //left
            if (!mapManager.GetTile((q.GetX()+1), q.GetY()).solid)
            {
                openL = successorNode(q, (q.GetX()+1), q.GetY(), playerX, playerY, openL, closeL);
            }
            closeL.Add(q);
            //right
            if (!mapManager.GetTile((q.GetX() - 1), q.GetY()).solid)
            {
                openL = successorNode(q, (q.GetX() - 1), q.GetY(), playerX, playerY, openL, closeL);
            }
            closeL.Add(q);
        }
        
        return null;
    }

    int[] reconstruct_path(Node node)
    {
        int[] temp = new int[2];
        temp[0] = node.GetX();
        temp[1] = node.GetY();
        if (node.GetParentCount() < 1)
        {
            temp = reconstruct_path(node.GetParent());
        }
        return temp;
    }

    List<Node> successorNode(Node q, int x, int y, int playerX, int playerY, List<Node> openL, List<Node> closeL)
    {
        bool openList = false;
        bool closeList = false;
        Node successor = new Node();
        successor = q;
        successor.SetX(x);
        successor.SetY(y);
        successor.SetParent(q);
        successor.SetG(q.GetG() + 1);
        float h = Mathf.Abs(successor.GetX() - playerX) + Mathf.Abs(successor.GetY() - playerY);
        successor.SetH(h);
        successor.SetF();

        foreach (Node o in openL)
        {
            if (o.GetF() == q.GetF())
            {
                openList = true;
            }
        }
        foreach (Node o in closeL)
        {
            if (o.GetF() == q.GetF())
            {
                closeList = true;
            }
        }
        if (openList || closeList)
        {
        }
        else
        {
            openL.Add(successor);
        }
        return openL;
    }
}
