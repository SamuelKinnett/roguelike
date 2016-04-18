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

	List<string> dungeonPrefix;
	List<string> dungeonFirstName;
	List<string> dungeonSecondName;
	List<string> dungeonSuffix;

	// Use this for initialization
	void Start ()
	{
		//The file manager will be used by both the main menu and the game scene.
		// Therefore, ensure that this will not be destroyed.
		DontDestroyOnLoad (this);
		LoadNPCData ();

		dungeonPrefix = new List<string> ();
		dungeonFirstName = new List<string> ();
		dungeonSecondName = new List<string> ();
		dungeonSuffix = new List<string> ();

		LoadDungeonNameData ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void LoadNPCData ()
	{
		TextAsset npcFile = new TextAsset ();
		npcFile = Resources.Load ("Data/NPC") as TextAsset;
		string[] npcStrings = npcFile.text.Split ('\n');

		int numberOfNPCs = int.Parse (npcStrings [0]);
		Debug.Log ("Loading " + numberOfNPCs + " NPCs");

		npcs = new List<NPC> ();

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

	public List<NPC> GetAllNPCS() {
		return npcs;
	}

	void LoadDungeonNameData() {
		TextAsset dungeonPreData = new TextAsset ();
		TextAsset dungeonFirstData = new TextAsset ();
		TextAsset dungeonSecondData = new TextAsset ();
		TextAsset dungeonSufData = new TextAsset ();

		dungeonPreData = Resources.Load ("Data/DungeonPrefix") as TextAsset;
		dungeonFirstData = Resources.Load ("Data/DungeonFirst") as TextAsset;
		dungeonSecondData = Resources.Load ("Data/DungeonSecond") as TextAsset;
		dungeonSufData = Resources.Load ("Data/DungeonSuffix") as TextAsset;

		string[] tempDungeonPre = dungeonPreData.text.Split ('\n');
		string[] tempDungeonFirst = dungeonFirstData.text.Split ('\n');
		string[] tempDungeonSecond = dungeonSecondData.text.Split ('\n');
		string[] tempDungeonSuf = dungeonSufData.text.Split ('\n');

		for (int temp = 0; temp < tempDungeonPre.Length; ++temp) {
			dungeonPrefix.Add (tempDungeonPre [temp]);
		}

		for (int temp = 0; temp < tempDungeonFirst.Length; ++temp) {
			dungeonFirstName.Add (tempDungeonFirst [temp]);
		}

		for (int temp = 0; temp < tempDungeonSecond.Length; ++temp) {
			dungeonSecondName.Add (tempDungeonSecond [temp]);
		}

		for (int temp = 0; temp < tempDungeonSuf.Length; ++temp) {
			dungeonSuffix.Add (tempDungeonSuf [temp]);
		}

		Debug.Log (dungeonPrefix.Count);
		Debug.Log (dungeonFirstName.Count);
		Debug.Log (dungeonSecondName.Count);
		Debug.Log (dungeonSuffix.Count);
	}

	public string GetDungeonPrefix() {
		return dungeonPrefix [(int)Mathf.Round((dungeonPrefix.Count - 1) * Random.value)];
	}

	public string GetDungeonFirstName() {
		return dungeonFirstName [(int)Mathf.Round((dungeonFirstName.Count - 1) * Random.value)];
	}

	public string GetDungeonSecondName() {
		return dungeonSecondName [(int)Mathf.Round((dungeonSecondName.Count - 1) * Random.value)];
	}

	public string GetDungeonSuffix() {
		return dungeonSuffix [(int)Mathf.Round((dungeonSuffix.Count - 1) * Random.value)];
	}

}
