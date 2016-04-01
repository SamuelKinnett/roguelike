using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

	public GameObject mainMenuPanel;

	public void StartGame ()
	{
		SceneManager.LoadScene ("Dungeon");
	}

	public void DisableMainMenu ()
	{
		mainMenuPanel.SetActive (false);
	}

}
