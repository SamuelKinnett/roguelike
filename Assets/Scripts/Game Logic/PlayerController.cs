using UnityEngine;
using System.Collections;


//Main structure for holding play stats

public class PlayerController : MonoBehaviour
{
	public struct stats
	{
		public int armour;
		//Main normal damage protection
		public int strength;
		// Main normal damage addition
		public int wisdom;
		//Magic armour + Magic strength (May be split later)
		public int agility;
		//Dodge chance (May become ranger stat later)
		public int hp;
		//Main health for player
		public int level;
		//Main player level, doesn't do a whole lot except allow for item and weapon equipping
	}

	public SpriteRenderer spriteRenderer;
	public GameObject obj_HitResultManager;

	stats playerStats;

	private float worldWidth, worldHeight;
	private HitResultManager hitResultManager;
	private Animator animator;
	float animationCooldown;
	Random rnd = new Random ();

	int x, y;

	// Use this for initialization
	void Start ()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		animator = this.GetComponent<Animator> ();
		hitResultManager = obj_HitResultManager.GetComponent<HitResultManager> ();
		worldWidth = spriteRenderer.bounds.size.x;
		worldHeight = spriteRenderer.bounds.size.y;
		//Starting player stats
		playerStats = new stats ();
		playerStats.armour = 10;
		playerStats.strength = 9;
		playerStats.wisdom = 1;
		playerStats.agility = 1;
		playerStats.hp = 25;
		playerStats.level = 1;
	}

	// Update is called once per frame
	void Update ()
	{
		if (animationCooldown < 0) {
			animationCooldown -= Time.deltaTime;
		} else {
			animator.SetBool ("moving", false);
		}
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
		this.x = x;
		this.y = y;
		animationCooldown = 0.1f;
		animator.SetBool ("moving", true);
	}

	/// <summary>
	/// Function for getting all player stats, mostly used for GUI elements.
	/// </summary>
	/// <returns>playerStats struct</returns>
	public stats GetAllPlayerStats ()
	{
		return playerStats;
	}

	public float GetSpriteSize ()
	{
		return worldWidth;
	}

	/// <summary>
	/// Calculates damage done to a player from enemy attack
	/// </summary>
	/// <param name="enemyStrength"></param>
	/// <returns>Damage done to player,function returns -1 is attack was dodged </returns>
	public bool PlayerHit (NPCStats enemyStats)
	{
		int dodgeChance = Random.Range (playerStats.agility, 101);
		HitResult hitResult = new HitResult ();

		if (dodgeChance == 100) {
			Debug.Log ("Player dodged attack!");
			return false;
		} else {
			float playerArmour = 0.2f * playerStats.armour;
			int damageRemoved = Mathf.CeilToInt (playerArmour); //Rounds up to nearest int value
			int damageDone = (int)enemyStats.damage[0] - damageRemoved;

			if (damageDone < 1) {
				playerStats.hp = playerStats.hp - 1; //removes damage from the players current hp
				Debug.Log ("Enemy dealt 1 damage to player!");
				hitResultManager.CreateHitResult (x, y, "- 1", worldWidth);
				if (playerStats.hp < 1) {
					Debug.Log ("Player is dead!");
					return true;
				} else {
					return false;
				}
			} else {
				playerStats.hp = playerStats.hp - damageDone;
				Debug.Log ("Enemy dealt " + damageDone + " to player!");
				hitResultManager.CreateHitResult (x, y, "- " + damageDone, worldWidth);
				if (playerStats.hp < 1) {
					Debug.Log ("Player is dead!");
					return true;
				} else {
					return false;
				}
			}
		}
	}

	/// <summary>
	/// Levels up the character, now just adds 1 to each of the player stats will be changed later. Adds 5 to health.
	/// </summary>
	public void PlayerLevelUp ()
	{
		playerStats.armour += 1;
		playerStats.strength += 1;
		playerStats.wisdom += 1;
		playerStats.agility += 1;
		playerStats.hp += 5;
		playerStats.level += 1;
	}

}
