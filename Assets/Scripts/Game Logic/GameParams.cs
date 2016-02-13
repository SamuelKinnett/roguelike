using UnityEngine;
using System.Collections;

public class GameParams : MonoBehaviour {

	public int numberOfFloors;
	public int mapWidth;
	public int mapHeight;
	public string paletteName;

	// Use this for initialization
	void Start () {
		//Stops the object from being destroyed between loading scenes
		//This allows us to pass variables to the game scene.
		Object.DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
