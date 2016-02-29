using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameGUIController : MonoBehaviour
{

	public GameObject obj_GameController;
	public GameObject obj_CurrentLevelText;

	private GameController gameController;
	private Text currentLevelText;

	// Use this for initialization
	void Start ()
	{
		gameController = obj_GameController.GetComponent<GameController> ();
		currentLevelText = obj_CurrentLevelText.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentLevelText.text = "Floor " + (gameController.GetCurrentFloor () + 1);
	}
}
