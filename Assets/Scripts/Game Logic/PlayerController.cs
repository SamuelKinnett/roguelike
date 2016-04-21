using UnityEngine;
using System.Collections;


//Main structure for holding play stats

public class PlayerController : MonoBehaviour
{
    #region Player stats, Item Stats, and Armour Stats Structs
    public struct stats
    {
        public int hp;
        //Main health for player
        public int maxHp;
        //Max hp a character has
        public int hunger;
        //Main player hunger value
        public int maxHunger;
        //Max hunger for player
        public int strength;
        // Main normal damage addition
        public int toughness;
        //Main normal damage protection
        public int agility;
        //Dodge chance (May become ranger stat later)
        public int level;
        //Main player level, doesn't do a whole lot except allow for item and weapon equipping
        public int dexterity;
        //Damage stat bonus for smaller weapons
    }

    public struct weaponCompetence //Main competance for weapons and armour
    {
        public int tinyBlade;
        public int smallBlade;
        public int largeBlade;
        public int smallBlunt;
        public int largeBlunt;
        public int shortBow;
        public int longBow;
        public int crossbow;
    }
    public struct armourCompetence
    {
        public int lightArmour;
        public int medArmour;
        public int heavyArmour;
        public int smallShield;
        public int medShield;
        public int heavyShield;
    }
    #endregion

    public SpriteRenderer spriteRenderer;
	public GameObject obj_HitResultManager;

	stats playerStats;
	weaponCompetence playerWeaponCompetence;
	armourCompetence playerArmourCompetence;

	private float worldWidth, worldHeight;
	private HitResultManager hitResultManager;
	private Animator animator;
	float animationCooldown;
	Random rnd = new Random ();
    float[] itemsEquipped;

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
		playerWeaponCompetence = new weaponCompetence ();
		CreatePlayer ();

	}
    /// <summary>
    /// Function for creating player (Needs argument for class type)
    /// </summary>
    public void CreatePlayer()
	{
		playerStats.toughness = 5;
		playerStats.strength = 5;
		playerStats.agility = 5;
		playerStats.hp = 25;
		playerStats.maxHp = 25;
		playerStats.hunger = 1000;
		playerStats.maxHunger = 1000;
		playerStats.level = 1;
		playerStats.dexterity = 5;

		playerWeaponCompetence.tinyBlade = 1;
		playerWeaponCompetence.smallBlade = 1;
		playerWeaponCompetence.largeBlade = 1;
		playerWeaponCompetence.smallBlunt = 1;
		playerWeaponCompetence.largeBlunt = 1;
		playerWeaponCompetence.shortBow = 1;
		playerWeaponCompetence.longBow = 1;
		playerWeaponCompetence.crossbow = 1;

		playerArmourCompetence.lightArmour = 1;
		playerArmourCompetence.medArmour = 1;
		playerArmourCompetence.heavyArmour = 1;
		playerArmourCompetence.smallShield = 1;
		playerArmourCompetence.medShield = 1;
		playerArmourCompetence.heavyShield = 1;
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
    #region Orginal Combat code
    public bool TempPlayerHit(NPCStats enemyStats)
    {
        int dodgeChance = Random.Range(playerStats.agility, 101);
        HitResult hitResult = new HitResult();

        if (dodgeChance == 100)
        {
            Debug.Log("Player dodged attack!");
            return false;
        }
        else
        {
            float playerArmour = 0.2f * playerStats.toughness;
            int damageRemoved = Mathf.CeilToInt(playerArmour); //Rounds up to nearest int value
            int damageDone = (int)enemyStats.damage[0] - damageRemoved;

            if (damageDone < 1)
            {
                playerStats.hp = playerStats.hp - 1; //removes damage from the players current hp
                Debug.Log("Enemy dealt 1 damage to player!");
                hitResultManager.CreateHitResult(x, y, "- 1", worldWidth);
                if (playerStats.hp < 1)
                {
                    Debug.Log("Player is dead!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                playerStats.hp = playerStats.hp - damageDone;
                Debug.Log("Enemy dealt " + damageDone + " to player!");
                hitResultManager.CreateHitResult(x, y, "- " + damageDone, worldWidth);
                if (playerStats.hp < 1)
                {
                    Debug.Log("Player is dead!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    } 
    #endregion

    /// <summary>
    /// Levels up the character, now just adds 1 to each of the player stats will be changed later. Adds 5 to health.
    /// </summary>
    public void PlayerLevelUp ()
	{
		playerStats.toughness += 1;
		playerStats.strength += 1;
		playerStats.agility += 1;
		playerStats.hp += 5;
		playerStats.level += 1;
	}


	

	public bool PlayerHit(NPCStats npcStats)
	{
        //Generates a random float between 0 and 1, then checks what attack will be selected. Sets damage coresponding to attack type.
        float npcDamage;
        float hitType = Random.Range(0.0f, 2.0f);
        if (hitType < npcStats.preference[0])
        {
            Debug.Log("Enemy slashed!");
            npcDamage = npcStats.damage[0];

        }
        else if (hitType >= npcStats.preference[0] && hitType < npcStats.preference[2])
        {
            Debug.Log("Enemy Bashed");
            npcDamage = npcStats.damage[1];
        }
        else
        {
            Debug.Log("Enemy pierced");
            npcDamage = npcStats.damage[2];
        }

        //Generates a random int to check for dodge chance.
        HitResult hitResult = new HitResult();
        int hitChance = Random.Range(Mathf.CeilToInt(npcStats.combatSkill / playerStats.agility), 101);

        if (hitChance == 100)
        {
            Debug.Log("Player dodged attack!");
            return false;
        }
        else
        {
            float playerDefence = playerStats.toughness / (100 - GetEquippedItems());
            int damageDone = (int)Mathf.CeilToInt(npcDamage) - Mathf.CeilToInt(playerDefence);
            playerStats.hp -= damageDone;
            hitResultManager.CreateHitResult(x, y, "- " + damageDone, worldWidth);
            Debug.Log("Player took " + damageDone + " damage!");
            if(playerStats.hp <= 0)
            {
                Debug.Log("Player is dead!");
                return true;
            }
            else
            {
                 return false;
            }
           
        }
	}

    /// <summary>
    /// Fonction needed for current equipped character items (Placeholder currently as there is no way of knowing what the player has equipped)
    /// </summary>
    /// <returns>Equipped armour and Weapons/returns>
    public float GetEquippedItems()
    {
        //Pls ignore, just so shit dont break (8
        float temp = 5;
        return temp;
    }

			

}
