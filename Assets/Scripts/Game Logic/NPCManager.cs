using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{

	public GameObject obj_MapManager;
	public GameObject obj_Node;
    
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


	// Use this for initialization
	void Start ()
	{
		worldWidth = spriteRenderer.bounds.size.x;
		worldHeight = spriteRenderer.bounds.size.y;

		mapManager = obj_MapManager.GetComponent<MapManager> ();
		visualRadius = 5;
	}

	public void CreateNPC (int[] stats, Sprite sprite)
	{
		worldNPC = new GameObject ();
		spriteRenderer = worldNPC.AddComponent<SpriteRenderer> ();

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


	public int[] GetStats ()
	{
		return stats;
	}

	void UpdateWorldPosition ()
	{
		Vector3 tempTransform = this.transform.position;
		tempTransform.x = x * spriteRenderer.bounds.size.x;
		tempTransform.y = y * spriteRenderer.bounds.size.y;
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

	public int[] Movement (int playerX, int playerY)
	{
		//A* psudocode -> http://web.mit.edu/eranki/www/tutorials/search/
		Node npc = obj_Node.GetComponent<Node> ();
		npc.Initilise (x, y, 0, 0, 0);
		Node q = npc;

		List<Node> openL = new List<Node> ();//list of nodes to check
		List<Node> closeL = new List<Node> ();//list of checked nodes
        
		openL.Add (npc);//add start node

		while (openL.Count < 0) {//nodes to search
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
        
		return null;
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
