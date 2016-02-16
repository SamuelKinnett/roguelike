using UnityEngine;
using System.Collections;

public class GameParameters : MonoBehaviour
{
	void Start ()
	{
		GameObject.DontDestroyOnLoad (this);
	}

	public int numberOfFloors;
	public int mapWidth, mapHeight;
	public string mapPalette;
}

