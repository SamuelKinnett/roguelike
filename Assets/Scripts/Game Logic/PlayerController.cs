using UnityEngine;
using System.Collections;

public enum stats
{
	attack,
	armour,
	magicAttack,
	magicArmour,
	hp,
	agility
}

public class PlayerController : MonoBehaviour
{

	public SpriteRenderer spriteRenderer;
	private float worldWidth, worldHeight;

	int[] playerStats = new int[6];

	// Use this for initialization
	void Start ()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		worldWidth = spriteRenderer.bounds.size.x;
		worldHeight = spriteRenderer.bounds.size.y;

		for (int i = 0; i < playerStats.Length; i++) {
			playerStats [i] = 10; //Temporary set value of all stats for testing TODO: Change to 0
		}
		playerStats [(int)stats.hp] = 25; //Sets starting HP for character to 25
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	/// <summary>
	/// Sets the position of the player object in the game world
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SetPosition (int x, int y)
	{
		Vector3 tempTrans = this.transform.position;
		tempTrans.x = x * worldWidth;
		tempTrans.y = y * worldHeight;
		this.transform.position = tempTrans;
	}

	//Function for retieving (All) player stats
	public int[] GetAllPlayerStats ()
	{
		return playerStats;
	}

	public int GetStat (stats stat)
	{
		return playerStats [(int)stat];
	}

	//Called when the player is hit with an attack. All calculation goes here.
	// Return true if the player is killed.
	public bool PlayerHit (int[] enemyStats)
	{
		int damageDone = enemyStats [(int)stats.attack] - playerStats [(int)stats.armour];
		playerStats [(int)stats.hp] -= damageDone;
		if (playerStats [(int)stats.hp] < 1) {
			return true;
		}
		return false;
	}
}
