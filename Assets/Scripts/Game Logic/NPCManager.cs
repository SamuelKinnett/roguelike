using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{

	public GameObject obj_MapManager;
    
	public SpriteRenderer spriteRenderer;
	private float worldWidth, worldHeight;
	private MapManager mapManager;
	private GameObject worldNPC;
	//The game object representing the NPC in worldspace
   
	//Main structure that holds the NPC's stats
	int[] stats = new int[6];

	int visualRadius;
	int x;
	//x co-ordinate
	int y;
	//y co-ordinate

	//Temporary Pathfinding Variables
	List<int[]> movementPath;
	int movementPathPosition;
	int pathDistance;
	int distanceToPlayer;


	// Use this for initialization
	void Start ()
	{
		worldWidth = spriteRenderer.bounds.size.x;
		worldHeight = spriteRenderer.bounds.size.y;

		visualRadius = 5;
	}

	public void CreateNPC (int[] stats, Sprite sprite)
	{
		worldNPC = new GameObject ();
		spriteRenderer = worldNPC.AddComponent<SpriteRenderer> ();
		mapManager = obj_MapManager.GetComponent<MapManager> ();

		int[] randomPosition = new int[2];
		randomPosition = mapManager.GetRandomPosition ();
		x = randomPosition [0];
		y = randomPosition [1];
		this.stats = stats;
		spriteRenderer.sprite = sprite;
		UpdateWorldPosition ();
	}

	//Called to destroy the game object associated with this NPC
	public void DestroyNPC ()
	{
		Destroy (worldNPC);
	}

	/// <summary>
	/// Returns true if the NPC can attack the player
	/// </summary>
	/// <returns><c>true</c> if this instance can attack the specified playerX playerY; otherwise, <c>false</c>.</returns>
	/// <param name="playerX">Player x.</param>
	/// <param name="playerY">Player y.</param>
	public bool CanAttack (int playerX, int playerY)
	{
		//check if can attack player
		if (playerX == x + 1 && playerY == y || playerX == x - 1 && playerY == y || playerX == x && playerY == y + 1 || playerX == x && playerY == y - 1) {
			return true;
		}
		return false;
	}

	public bool NPCHit (int[] playerStats)
	{
		int damageDone = playerStats [(int)Stats.attack] - stats [(int)Stats.armour];
		stats [(int)Stats.hp] -= damageDone;
		if (stats [(int)Stats.hp] < 1) {
			return true;
		}
		return false;
	}

	public int[] GetStats ()
	{
		return stats;
	}

	public int[] GetPosition ()
	{
		return new int[2]{ x, y };
	}

	void UpdateWorldPosition ()
	{
		Vector3 tempTransform = worldNPC.transform.position;
		tempTransform.x = x * spriteRenderer.bounds.size.x;
		tempTransform.y = y * spriteRenderer.bounds.size.y;
		tempTransform.z = -1;
		worldNPC.transform.position = tempTransform;
	}

	#region movement

	/// <summary>
	/// Moves this particular NPC. Should always occur after checking if the NPC can attack.
	/// </summary>
	/// <param name="playerX">Player x.</param>
	/// <param name="playerY">Player y.</param>
	public void Move (int playerX, int playerY)
	{
		//visual detection
		if (playerX >= (x - visualRadius) && playerX <= (x + visualRadius) && playerY >= (y - visualRadius) && playerY <= (y + visualRadius)) {
			int[] position = Movement (playerX, playerY);//returns int[] of movment position
			if (position != null) {
				x = position [0];
				y = position [1];
				UpdateWorldPosition ();
			}
		}
	}

	int[] Pathfind (int targetX, int targetY)
	{
		List<int[]> openList = new List<int[]> ();
		List<int[]> closedList = new List<int[]> ();
		int[,] visitedSquares = new int[mapManager.mapWidth, mapManager.mapHeight];
		int F;
		int G;
		int H;
		int openListEntryNumber;
		int closedListEntryNumber;
		int parentID;
		int desiredID;
		bool canMove;

		int currentX;
		int currentY;

		currentX = x;
		currentY = y;

		movementPath = new List<int[]> ();

		pathDistance = 0;

		visitedSquares [currentX, currentY] = 1;

		int[] newEntry = new int[2];
		newEntry [0] = currentX;
		newEntry [1] = currentY;

		closedList.Add (newEntry);

		openListEntryNumber = -1;
		closedListEntryNumber = 0;
		G = 10;

		while ((currentX != targetX) || (currentY != targetY)) {

			canMove = false;

			if (!mapManager.GetTile (currentX + 1, currentY).solid) {
				if (visitedSquares [currentX + 1, currentY] == 0) {
					openListEntryNumber += 1;
					int[] openListEntry = new int[4];
					openListEntry [0] = currentX + 1;
					openListEntry [1] = currentY;
					openListEntry [2] = parentID;
					H = Mathf.Abs ((currentX + 1) - targetX) + Mathf.Abs (currentY - targetY);
					F = G + H;
					openListEntry [3] = F;
					openList.Add (openListEntry);
					canMove = true;
				}
			}

			if (solidMap [CurrentX - 1, CurrentY] != 1) {
				if (visitedSquares [CurrentX - 1, CurrentY] == 0) {
					openListEntryNumber += 1;
					int[] openListEntry = new int[4];
					openListEntry [0] = currentX - 1;
					openListEntry [1] = currentY;
					openListEntry [2] = parentID;
					H = Mathf.Abs ((currentX - 1) - targetX) + Mathf.Abs (currentY - targetY);
					F = G + H;
					openListEntry [3] = F;
					openList.Add (openListEntry);
					canMove = True;
				}
			}

			if (solidMap [CurrentX, CurrentY + 1] != 1) {
				if (visitedSquares [CurrentX, CurrentY + 1] == 0) {
					openListEntryNumber += 1;
					int[] openListEntry = new int[4];
					openListEntry [0] = currentX;
					openListEntry [1] = currentY + 1;
					openListEntry [2] = parentID;
					H = Mathf.Abs (currentX - targetX) + Mathf.Abs ((currentY + 1) - targetY);
					F = G + H;
					openListEntry [3] = F;
					openList.Add (openListEntry);
					canMove = True;
				}
			}

			if (solidMap [CurrentX, CurrentY - 1] != 1) {
				if (visitedSquares [CurrentX, CurrentY - 1] == 0) {
					openListEntryNumber += 1;
					int[] openListEntry = new int[4];
					openListEntry [0] = currentX;
					openListEntry [1] = currentY - 1;
					openListEntry [2] = parentID;
					H = Mathf.Abs (currentX - targetX) + Mathf.Abs ((currentY - 1) - targetY);
					F = G + H;
					openListEntry [3] = F;
					openList.Add (openListEntry);
					canMove = True;
				}
			}

			if (!canMove) {
				break;
			}

			desiredID = 0;

			for (int count = 0; count <= openListEntryNumber; ++count) {
				if (openList [count] [3] < openList [desiredID] [3]) {
					desiredID = count;
				}
			}

			closedListEntryNumber += 1;

			int[] closedListEntry = new int[2];

			closedListEntry [0] = openList [desiredID] [0];
			closedListEntry [1] = openList [desiredID] [1];

			closedList.Add (closedListEntry);

			currentX = openList [desiredID] [0];
			currentY = openList [desiredID] [1];

			visitedSquares [currentX, currentY] = 1;

			openListEntryNumber = -1;
		}

		int[] movementPathEntry = new int[2];

		for (int count = 0; count <= closedListEntryNumber; ++count) {
			movementPathEntry [count] [0] = closedList [count] [0];
			movementPathEntry [count] [1] = closedList [count] [1];
			pathDistance += 1;
		}

		movementPathPosition = 0;

		distanceToPlayer = pathDistance;
	}

	public int[] Movement (int playerX, int playerY)
	{
		//A* psudocode -> http://web.mit.edu/eranki/www/tutorials/search/
		Node npc = new Node ();
		npc.Initilise (x, y, 0, 0, 0);
		Node q = npc;

		List<Node> openL = new List<Node> ();//list of nodes to check
		List<Node> closeL = new List<Node> ();//list of checked nodes
        
		openL.Add (npc);//add start node

		while (openL.Count > 0) {//nodes to search
			float qf = 100;
			foreach (Node n in openL) {
				if (qf > n.GetF ()) {
					qf = n.GetF ();
					q = n;
					openL.RemoveAt (openL.IndexOf (n));  
				}
			}
			if (q.GetX () == playerX && q.GetY () == playerY) {//check if route is found
				return reconstruct_path (q);
			}
			//search routes
			//up
			if (!mapManager.GetTile (q.GetX (), (q.GetY () + 1)).solid) {
				openL = successorNode (q, q.GetX (), (q.GetY () + 1), playerX, playerY, openL, closeL);
			}

			//down
			if (!mapManager.GetTile (q.GetX (), (q.GetY () - 1)).solid) {
				openL = successorNode (q, q.GetX (), (q.GetY () - 1), playerX, playerY, openL, closeL);
			}
            
			//left
			if (!mapManager.GetTile ((q.GetX () + 1), q.GetY ()).solid) {
				openL = successorNode (q, (q.GetX () + 1), q.GetY (), playerX, playerY, openL, closeL);
			}
           
			//right
			if (!mapManager.GetTile ((q.GetX () - 1), q.GetY ()).solid) {
				openL = successorNode (q, (q.GetX () - 1), q.GetY (), playerX, playerY, openL, closeL);
			}
			closeL.Add (q);
		}
        
		int[] returnArray = new int[2];
		returnArray [0] = closeL [1].GetX ();
		returnArray [1] = closeL [1].GetY ();
		return returnArray;
	}

	int[] reconstruct_path (Node node)
	{
		int[] temp = new int[2];
		temp [0] = node.GetX ();
		temp [1] = node.GetY ();
		if (node.GetParentCount () < 1) {
			temp = reconstruct_path (node.GetParent ());
		}
		return temp;
	}

	List<Node> successorNode (Node q, int x, int y, int playerX, int playerY, List<Node> openL, List<Node> closeL)
	{
		bool openList = false;
		bool closeList = false;
		Node successor = new Node ();
		successor = q;
		successor.SetX (x);//set  new co-ordinates
		successor.SetY (y);
		successor.SetParent (q);//set parent node
		successor.SetG (q.GetG () + 1);//set cost to get to node
		float h = Mathf.Abs (successor.GetX () - playerX) + Mathf.Abs (successor.GetY () - playerY);
		successor.SetH (h);//set heuristic cost to get to node
		successor.SetF ();

		foreach (Node o in openL) {
			if (o.GetF () == q.GetF ()) {
				openList = true;
			}
		}
		foreach (Node o in closeL) {
			if (o.GetF () == q.GetF ()) {
				closeList = true;
			}
		}
		if (openList || closeList) {
		} else {
			openL.Add (successor);
		}
		return openL;
	}

	#endregion movement

}
