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

public class Player : MonoBehaviour {
    //Main structure that holds player stats
    int[] playerStats = new int[6];

    // Use this for initialization
    void Start () {
        for(int i = 0; i < playerStats.Length; i++)
        {
            playerStats[i] = 2; //Temporary set value of all stats for testing TODO: Change to 0
        }
        playerStats[(int)stats.hp] = 25; //Sets starting HP for character to 25
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //Function for retieving (All) player stats
    public int[] GetAllPlayerStats()
    {
        return playerStats;
    }

    //Calculation for how much damage is done, requires array of enemy stats
    public int PlayerHit(int[] enemyStats)
    {
        int damageDone = playerStats[(int)stats.armour] - enemyStats[(int)stats.attack];
        return damageDone;
    }
}
