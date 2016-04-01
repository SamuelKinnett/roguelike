using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum ItemType
{
	TinyBlade,
	SmallBlade,
	LargeBlade,
	SmallBlunt,
	LargeBlunt,
	LightShield,
	MediumShield,
	HeavyShield,
	LightArmour,
	MediumArmour,
	HeavyArmour,
	Food,
	Ammunition,
	Trinket
}

public struct NPCStats
{
	public int hp;
	public float combatSkill;
	public float[] damage;
	public float[] defence;
	public float[] preference;
	public float agility;
	public float perception;
	public bool aggressive;
	public bool hostile;
}

public struct NPC
{
	public string name;
	public int[] levelRange;
	public NPCStats stats;
	public float baseEXP;
	public float EXPModifier;
	public float[] biasValues;
	public List<ItemType> drops;
	public int[] dropLevelRange;
}

public class FileManager : MonoBehaviour
{

	List<NPC> npcs;

	// Use this for initialization
	void Start ()
	{
		//The file manager will be used by both the main menu and the game scene.
		// Therefore, ensure that this will not be destroyed.
		DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void LoadNPCData ()
	{
		TextAsset npcFile = Resources.Load ("Data\\NPC.txt") as TextAsset;
		string[] npcStrings = npcFile.text.Split ('\n');

		int numberOfNPCs = int.Parse (npcStrings [0]);
		Debug.Log ("Loading " + numberOfNPCs + " NPCs");

		for (int i = 1; i < numberOfNPCs + 1; ++i) {
			NPC npc = new NPC ();
			string[] npcData = npcStrings [i].Split (';');

			npc.name = npcData [0];
			npc.levelRange = new int[2];
			npc.levelRange [0] = int.Parse (npcData [1]);
			npc.levelRange [1] = int.Parse (npcData [2]);
			npc.baseEXP = float.Parse (npcData [3]);
			npc.EXPModifier = float.Parse (npcData [4]);
			npc.dropLevelRange = new int[2];
			npc.dropLevelRange [0] = int.Parse (npcData [5]);
			npc.dropLevelRange [1] = int.Parse (npcData [6]);

			//Load in the NPC's stats

			npc.stats = new NPCStats ();
			npc.stats.hp = int.Parse (npcData [7]);
			npc.stats.combatSkill = float.Parse (npcData [8]);
			npc.stats.damage = new float[3];
			npc.stats.damage [0] = float.Parse (npcData [9]);
			npc.stats.damage [1] = float.Parse (npcData [10]);
			npc.stats.damage [2] = float.Parse (npcData [11]);
			npc.stats.defence = new float[3];
			npc.stats.defence [0] = float.Parse (npcData [12]);
			npc.stats.defence [1] = float.Parse (npcData [13]);
			npc.stats.defence [2] = float.Parse (npcData [14]);
			npc.stats.preference = new float[3];
			npc.stats.preference [0] = float.Parse (npcData [15]);
			npc.stats.preference [1] = float.Parse (npcData [16]);
			npc.stats.preference [2] = float.Parse (npcData [17]);
			npc.stats.agility = float.Parse (npcData [18]);
			npc.stats.perception = float.Parse (npcData [19]);
			npc.stats.aggressive = bool.Parse (npcData [20]);
			npc.stats.hostile = bool.Parse (npcData [21]);

			//Load in the bias values

			npc.biasValues = new float[10];

			for (int c = 0; c < 10; ++c)
				npc.biasValues [c] = float.Parse (npcData [22 + c]);

			//Load in the drop types

			if (npcData [32] != "END") {
				npc.drops = new List<ItemType> ();
				int curIndex = 32;
				while (npcData [curIndex] != "END") {
					npc.drops.Add ((ItemType)System.Enum.Parse (typeof(ItemType), npcData [curIndex]));
					++curIndex;
				}
			}

			npcs.Add (npc);
		}
	}

}
