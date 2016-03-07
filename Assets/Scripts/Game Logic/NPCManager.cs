using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{

    public struct stats
    {
        public int armour; //Main normal damage protection
        public int strength; // Main normal damage addition
        public int wisdom; //Magic armour + Magic strength (May be split later)
        public int agility; //Dodge chance (May become ranger stat later)
        public int hp; //Main health for player
        public int level; //Main player level, doesn't do a whole lot except allow for item and weapon equipping
        public string playerType; //Not Used for NPC
    }

    stats enemyStats = new stats();
    public GameObject obj_MapManager;

    public SpriteRenderer spriteRenderer;
    private float worldWidth, worldHeight;
    private MapManager mapManager;
    private GameObject worldNPC;
    //The game object representing the NPC in worldspace

    //Main structure that holds the NPC's stats

    int visualRadius;
    //how far npc can see
    int x;
    //x co-ordinate
    int y;
    //y co-ordinate

    bool[] adjacentEntities = new bool[4];


    // Use this for initialization
    void Start()
    {
        worldWidth = spriteRenderer.bounds.size.x;
        worldHeight = spriteRenderer.bounds.size.y;

        visualRadius = 5;
    }

    public void CreateNPC(stats NPCStats, Sprite sprite)
    {
        worldNPC = new GameObject();
        spriteRenderer = worldNPC.AddComponent<SpriteRenderer>();
        mapManager = obj_MapManager.GetComponent<MapManager>();

        int[] randomPosition = new int[2];
        randomPosition = mapManager.GetRandomPosition();
        x = randomPosition[0];
        y = randomPosition[1];
        enemyStats = NPCStats;
        spriteRenderer.sprite = sprite;
        UpdateWorldPosition();
    }

    //Called to destroy the game object associated with this NPC
    public void DestroyNPC()
    {
        Destroy(worldNPC);
		Destroy (this);
    }

    /// <summary>
    /// Returns true if the NPC can attack the player
    /// </summary>
    /// <returns><c>true</c> if this instance can attack the specified playerX playerY; otherwise, <c>false</c>.</returns>
    /// <param name="playerX">Player x.</param>
    /// <param name="playerY">Player y.</param>
    public bool CanAttack(int playerX, int playerY)
    {
        //check if can attack player
        if (playerX == x + 1 && playerY == y || playerX == x - 1 && playerY == y || playerX == x && playerY == y + 1 || playerX == x && playerY == y - 1)
        {
            return true;
        }
        return false;
    }

    public bool NPCHit(PlayerController.stats playerStats)
    {
        int dodgeChance = Random.Range(enemyStats.agility, 101);

        if (dodgeChance == 100)
        {
            Debug.Log("Enemy dodged attack!");
            return false;
        }
        else
        {
            float enemyArmour = 0.2f * enemyStats.armour;
            int damageRemoved = Mathf.CeilToInt(enemyArmour); //Rounds up to nearest int value
            int damageDone = playerStats.strength - damageRemoved;

            if (damageDone < 1)
            {
                enemyStats.hp = enemyStats.hp - 1; //removes damage from the players current hp
                Debug.Log("Player dealt 1 damage to enemy!");
                if (enemyStats.hp < 1)
                {
                    Debug.Log("Enemy is dead!");
					DestroyNPC ();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                enemyStats.hp = enemyStats.hp - damageDone;
                Debug.Log("Player dealt " + damageDone + " to enemy!");
                if (enemyStats.hp < 1)
                {
                    Debug.Log("Enemy is dead!");
					DestroyNPC ();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public stats GetAllEnemyStats()
    {
        return enemyStats;
    }

    public int[] GetPosition()
    {
        return new int[2] { x, y };
    }

    void UpdateWorldPosition()
    {
        Vector3 tempTransform = worldNPC.transform.position;
        tempTransform.x = x * spriteRenderer.bounds.size.x;
        tempTransform.y = y * spriteRenderer.bounds.size.y;
        tempTransform.z = -1;
        worldNPC.transform.position = tempTransform;

        switch (mapManager.GetTile(x, y).visibility)
        {

            case TileVisibility.seen:
                //The tile has previously been seen, but is now not visible,
                //so tint it to be darker
                spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
                spriteRenderer.enabled = false;
                break;

            case TileVisibility.unseen:
                //For whatever reason, the tile is now unknown to the player.
                //Make it invisible.
                spriteRenderer.enabled = false;
                break;

            case TileVisibility.visible:
                //The tile is currently visible, apply tinting based on the intensity
                // value.
                spriteRenderer.color = new Color(mapManager.GetTile(x, y).intensity,
                    mapManager.GetTile(x, y).intensity,
                    mapManager.GetTile(x, y).intensity);
                spriteRenderer.enabled = true;
                break;
        }
    }

    #region movement

    /// <summary>
    /// Moves this particular NPC. Should always occur after checking if the NPC can attack.
    /// </summary>
    /// <param name="playerX">Player x.</param>
    /// <param name="playerY">Player y.</param>
    public void Move(int playerX, int playerY)
    {
		//visual detection
		float distanceToPlayer = 0;
		distanceToPlayer = Mathf.Sqrt (Mathf.Pow ((x - playerX), 2) + Mathf.Pow ((y - playerY), 2));
		Debug.Log ("Distance to player: " + distanceToPlayer);
		if ( distanceToPlayer < 7) {
			int[] position = Movement (playerX, playerY);//returns int[] of movment position
			if (position != null) {
				x = position [0];
				y = position [1];
				UpdateWorldPosition ();
			}
		} else {
			//Pathfind to a random position on the map
			UpdateWorldPosition ();
		}
		//*/
    }
    //James' new A* pathfinding

    public int[] Movement(int playerX, int playerY)
    {
        //A* psudocode -> http://web.mit.edu/eranki/www/tutorials/search/

        List<int[]> openL = new List<int[]>();//list of nodes to check
        List<int[]> closeL = new List<int[]>();//list of checked nodes
        int index = 0;
        int[] noChange = new int[2];
        noChange[0] = x;
        noChange[1] = y;
        int[] startNode = new int[7];
        startNode[0] = 0;//0 is index
        startNode[1] = x;//1 is x
        startNode[2] = y;//2 is y
        startNode[3] = 0;//3 is parent
        startNode[4] = 0;//4 is movement count g
        startNode[5] = 0;//5 is h
        startNode[6] = 0;//7 is f
        int[] q = startNode;
        openL.Add(startNode);//add start node

        while (openL.Count > 0)//nodes to search
        {
            float qf = 100;

            foreach (int[] n in openL)
            {
                if (qf > n[5])
                {
                    qf = n[5];
                    q = n;
                    index = openL.IndexOf(n);
                }
            }
            openL.RemoveAt(index);
            closeL.Add(q);

            int[] lNode = new int[7];
            lNode = closeL[closeL.Count - 1];
            //PrintList(openL, "openRemoved");
            if (q[1] == playerX && q[2] == playerY)//check if route is found
            {
                Debug.Log("Found path");

                foreach (int[] n in closeL)
                {
                    Debug.Log("Node list:" + n[1] + " " + n[2]);
                }

                //return noChange;

                int parent = lNode[3];
                //Console.WriteLine("LAst node Node list:" + lNode[1] + " " + lNode[2]);
                int[] temp = reconstruct_path(closeL, parent);
                Debug.Log("Return:" + temp[0] + " " + temp[1]);
                return temp;
            }
            //Console.WriteLine("Q:" + q[1] + " " + q[2]);
            //search routes
            //up
            int tempX = q[1];
            int tempY = q[2];
            int addToIndex = 1;
            
			//if (mapManager.GetTile(tempX, tempY + 1) != null)
			if (!mapManager.GetTile(tempX, (tempY + 1)).solid)
            //if (tempY + 1 < 19)
            {
                openL = successorNode(q, tempX, (tempY + 1), playerX, playerY, openL, closeL, lNode[0] + addToIndex);
                addToIndex++;
            }
            foreach (int[] n in openL)
            {
                //Console.WriteLine("open list:" + n[1] + " " + n[2]);
            }
            //PrintList(openL, "open");

            //down
			//if (mapManager.GetTile(tempX, tempY - 1) != null)
			if (!mapManager.GetTile(tempX, (tempY - 1)).solid)
            //if (tempY - 1 >= 0)
            {
                openL = successorNode(q, tempX, (tempY - 1), playerX, playerY, openL, closeL, lNode[0] + addToIndex);
                addToIndex++;
            }
            //PrintList(openL, "open");
            //left
			//if (mapManager.GetTile(tempX + 1, tempY) != null)
            if (!mapManager.GetTile((tempX + 1), tempY).solid)
            //if (tempX + 1 < 19)
            {
                openL = successorNode(q, (tempX + 1), tempY, playerX, playerY, openL, closeL, lNode[0] + addToIndex);
                addToIndex++;
            }
            //PrintList(openL, "open");
            //right
			//if (mapManager.GetTile(tempX - 1, tempY) != null)
            if (!mapManager.GetTile((tempX - 1), tempY).solid)
            //if (tempX - 1 >= 0)
            {
                openL = successorNode(q, (tempX - 1), tempY, playerX, playerY, openL, closeL, lNode[0] + addToIndex);
                addToIndex++;
            }
            //PrintList(openL, "open");


            //PrintList(closeL, "close");
            //map[q.GetX(), q.GetY()] = "S";
            //printMap();
        }

        return noChange;
    }

    int[] reconstruct_path(List<int[]> path, int index)
    {
        int[] temp = new int[2];
        temp[0] = 0;
        temp[1] = 0;

        foreach (int[] n in path)
        {

            if (n[0] == index)
            {
                Debug.Log("Node " + n[0] + " reconstruct:" + n[1] + " " + n[2] + " Parent node " + n[3]);
                temp[0] = n[1];
                temp[1] = n[2];
                if (n[4] > 1)
                {
                    temp = reconstruct_path(path, n[3]);
                }
            }
        }


        return temp;
    }

    List<int[]> successorNode(int[] q, int x, int y, int targetX, int targetY, List<int[]> openL, List<int[]> closeL, int newIndex)
    {
        bool openList = false;
        bool closeList = false;

        int[] successsor = new int[8];
        successsor[0] = newIndex;//0 is index
        successsor[1] = x;//1 is x
        successsor[2] = y;//2 is y
        successsor[3] = q[0];//3 is parent
        successsor[4] = q[4] + 1;//4 is movement count g
        successsor[5] = Mathf.Abs(successsor[1] - targetX) + Mathf.Abs(successsor[2] - targetY); //5 is h
        successsor[6] = successsor[4] + successsor[5];//7 is f

        Debug.Log("S:" + successsor[1] + " " + successsor[2]);
        foreach (int[] o in openL)
        {
            if (o[1] == successsor[1] && o[2] == successsor[2] && o[6] < successsor[6])
            {
                openList = true;
            }
        }
        foreach (int[] o in closeL)
        {
            if (o[1] == successsor[1] && o[2] == successsor[2] && o[6] < successsor[6])
            {
                closeList = true;
            }
        }
        if (openList || closeList)
        {
        }
        else
        {
            openL.Add(successsor);
        }
        return openL;
    }

    //James' old pathfinding
    public int[] Movements(int playerX, int playerY)
    {
        //A* psudocode -> http://web.mit.edu/eranki/www/tutorials/search/
        Node npc = new Node();
        npc.Initilise(x, y, 0, 0, 0);
        Node q = npc;

        List<Node> openL = new List<Node>();//list of nodes to check
        List<Node> closeL = new List<Node>();//list of checked nodes

        openL.Add(npc);//add start node

        while (openL.Count > 0)
        {//nodes to search
            float qf = 100;
            foreach (Node n in openL)
            {
                if (qf > n.GetF())
                {
                    qf = n.GetF();
                    q = n;
                    openL.RemoveAt(openL.IndexOf(n));
                }
            }
            if (q.GetX() == playerX && q.GetY() == playerY)
            {//check if route is found
                return reconstruct_path(q);
            }
            //search routes
            //up
            if (!mapManager.GetTile(q.GetX(), (q.GetY() + 1)).solid)
            {
                openL = successorNode(q, q.GetX(), (q.GetY() + 1), playerX, playerY, openL, closeL);
            }

            //down
            if (!mapManager.GetTile(q.GetX(), (q.GetY() - 1)).solid)
            {
                openL = successorNode(q, q.GetX(), (q.GetY() - 1), playerX, playerY, openL, closeL);
            }

            //left
            if (!mapManager.GetTile((q.GetX() + 1), q.GetY()).solid)
            {
                openL = successorNode(q, (q.GetX() + 1), q.GetY(), playerX, playerY, openL, closeL);
            }

            //right
            if (!mapManager.GetTile((q.GetX() - 1), q.GetY()).solid)
            {
                openL = successorNode(q, (q.GetX() - 1), q.GetY(), playerX, playerY, openL, closeL);
            }
            closeL.Add(q);
        }

        int[] returnArray = new int[2];
        returnArray[0] = closeL[1].GetX();
        returnArray[1] = closeL[1].GetY();
        return returnArray;
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
        successor.SetX(x);//set  new co-ordinates
        successor.SetY(y);
        successor.SetParent(q);//set parent node
        successor.SetG(q.GetG() + 1);//set cost to get to node
        float h = Mathf.Abs(successor.GetX() - playerX) + Mathf.Abs(successor.GetY() - playerY);
        successor.SetH(h);//set heuristic cost to get to node
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

    #endregion movement

    public bool CheckCollision(int testX, int testY)
    {
        return ((x == testX) && (y == testY));
    }

    //Updates the adjacentEntities array, allowing the npc to check if movement is possible
    public void UpdateAdjacentEntities(bool[] adjacentEntities)
    {
        this.adjacentEntities = adjacentEntities;
    }
}
