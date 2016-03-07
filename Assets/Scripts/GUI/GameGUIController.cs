using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameGUIController : MonoBehaviour
{

	public GameObject obj_GameController;
	public GameObject obj_CurrentLevelText;
	public GameObject obj_CurrentHealthText;
	public GameObject obj_PlayerController;

	private GameController gameController;
	private PlayerController playerController;
	private Text currentLevelText;
	private Text currentHealthText;

	// Use this for initialization
	void Start ()
	{
		gameController = obj_GameController.GetComponent<GameController> ();
		currentLevelText = obj_CurrentLevelText.GetComponent<Text> ();
		currentHealthText = obj_CurrentHealthText.GetComponent<Text> ();
		playerController = obj_PlayerController.GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentLevelText.text = "Floor " + (gameController.GetCurrentFloor () + 1);
		currentHealthText.text = "HP: " + playerController.GetAllPlayerStats().hp;
	}
}
