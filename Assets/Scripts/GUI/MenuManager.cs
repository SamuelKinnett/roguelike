using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    
	public GameObject menuPanel;
    public GameObject canvas;
    public Font myFont;
    public GameObject obj_GameController;

    private GameController gameController;

    private bool helpPress = false;
    private bool soundPress = false;
    private bool brightPress = false;
    private bool aboutPress = false;

    private bool samPress = false;
    private bool conorPress = false;
    private bool jamesPress = false;
    private bool warwickPress = false;

    private bool pauseMenuActive;
    private bool areYouSure;

    private float brightness = 50;

    private GUIStyle guiStyle = new GUIStyle();
    //This void finds the name of the button that has been clicked and finds the corresponding 'if' statement
    //E.G. if the name of the button that was pressed == "LoadGameButton", then load the game
    //so the Unity management remains cleaner regardless of how many buttons we have
    public void ButtonSelector()
    {
        //NEW GAME
        if (EventSystem.current.currentSelectedGameObject.name == "NewGameButton")
        {
            SceneManager.LoadScene("Dungeon");
        }
        //LOAD GAME
        if (EventSystem.current.currentSelectedGameObject.name == "LoadGameButton")
        {
            //TODO: SAM! Your loading code goes here! The saving code will be linked with the pause menu
            menuPanel.SetActive(false);
        }     
        //OPTIONS
        if (EventSystem.current.currentSelectedGameObject.name == "OptionsButton")
        {
            SceneManager.LoadScene("Options Menu");
        }
        //QUIT
        if (EventSystem.current.currentSelectedGameObject.name == "QuitButton")
        {
            canvas.SetActive(false);
            Application.Quit(); //--doesn't seem to work...
        }
        //ABOUT
        if (EventSystem.current.currentSelectedGameObject.name == "AboutButton")
        {
            menuPanel.SetActive(false);
            Camera.main.backgroundColor = new Color(0, 0, 0);
            aboutPress = true;
        }
        //SOUND
        if (EventSystem.current.currentSelectedGameObject.name == "SoundButton")
        {
            menuPanel.SetActive(false);
            Camera.main.backgroundColor = new Color(0, 0, 0);
            soundPress = true;
        }
        //BRIGHTNESS
        if (EventSystem.current.currentSelectedGameObject.name == "BrightnessButton")
        {
            menuPanel.SetActive(false);
            Camera.main.backgroundColor = new Color(0, 0, 0);
            brightPress = true;
        }
        //BACK
        if (EventSystem.current.currentSelectedGameObject.name == "BackButton")
        {
            SceneManager.LoadScene("Main Menu");
        }
        //HELP
        if (EventSystem.current.currentSelectedGameObject.name == "HelpButton")
        {
            menuPanel.SetActive(false);
            Camera.main.backgroundColor = new Color(0, 0, 0);
            helpPress = true;
        }
    }

    void OnGUI()
    {
        //HELP BUTTON PRESSED
        if (helpPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle.
            GUI.Label(new Rect(380, 30, 60, 20), "HELP", guiStyle);
            //Change font size for description
            guiStyle.fontSize = 16;
            //Create description label. Add guiStyle. 
            GUI.Label(new Rect(40, 60, 60, 20), "\nUse the arrow keys to move around!\n\nPress 'H' to attack!\n\nPress 'M' to view the map!\n\nPress '.' to zoom in!\n\nPress ',' to zoom out!", guiStyle);
            //Create a button reading "Back" and when it is clicked, go back to the Options menu
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                menuPanel.SetActive(true);
                Camera.main.backgroundColor = new Color32(0, 88, 15, 5);
                helpPress = false;
            }
        }
        //SOUND BUTTON PRESSED
        if (soundPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle. 
            GUI.Label(new Rect(250, 30, 60, 20), "Sound Options", guiStyle);
            //Change to small text size
            guiStyle.fontSize = 16;
            //TODO: Create buttons for turning music on and off (mute/unmute)
            //TODO: Create slider for increasing/decreasing volume of track
            //TODO: Possibly create checkbox list of available tracks, where the user can tick the ones they want to hear in-game?
            //Creates a button reading "Back" and when it is clicked, go back to the Options menu
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                menuPanel.SetActive(true);
                Camera.main.backgroundColor = new Color32(0, 88, 15, 5);
                soundPress = false;
            }
        }
        //BRIGHTNESS BUTTON PRESSED
        if (brightPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle. 
            GUI.Label(new Rect(160, 30, 60, 20), "Brightness Options", guiStyle);
            //Change to small text size
            guiStyle.fontSize = 16;
            //TODO: Make slider actually do something in regards to game brightness, rather than sitting their looking all cool
            //like "Hey, I'm a slider, I'll slide on over to you in a minute". Bastard.
            brightness = GUI.HorizontalSlider(new Rect(160, 130, 580, 30), brightness, 0.0F, 100.0F);
            brightness = Mathf.RoundToInt(brightness);
            GUI.Label(new Rect(140, 130, 100, 30), brightness.ToString(), guiStyle);
            //Creates a button reading "Back" and when it is clicked, go back to the Options menu
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                menuPanel.SetActive(true);
                Camera.main.backgroundColor = new Color32(0, 88, 15, 5);
                brightPress = false;
            }
        }
        //ABOUT BUTTON PRESSED
        if (aboutPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle.
            GUI.Label(new Rect(320, 30, 60, 20), "About Us", guiStyle);
            //Change font size for description
            guiStyle.fontSize = 16;
            //Create description label. Add guiStyle. 
            GUI.Label(new Rect(10, 100, 60, 20), "This game was made by 4 students at Derby University,\nwho strove to conquer all coursework set against them!", guiStyle);
            //Create buttons for each person, with their names, and link it with their bios (on a seperate 'page')
            if (GUI.Button(new Rect(50, 180, 100, 50), "Sam", guiStyle))
            {
                samPress = true;
                aboutPress = false;
            }
            if (GUI.Button(new Rect(250, 180, 100, 50), "Conor", guiStyle))
            {
                conorPress = true;
                aboutPress = false;
            }
            if (GUI.Button(new Rect(450, 180, 100, 50), "James", guiStyle))
            {
                jamesPress = true;
                aboutPress = false;
            }
            if (GUI.Button(new Rect(650, 180, 100, 50), "Warwick", guiStyle))
            {
                warwickPress = true;
                aboutPress = false;
            }
            //Create a button reading "Back" and when it is clicked, go back to the Options menu
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                menuPanel.SetActive(true);
                Camera.main.backgroundColor = new Color32(0, 88, 15, 5);
                aboutPress = false;
            }
        }
        //NOTE: ALL LINES WITHIN BIOS CANNOT BE MORE THAN 54 CHARS LONG
        //SAM BUTTON PRESSED (Expansion of AboutPress)
        if (samPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle.
            GUI.Label(new Rect(220, 30, 60, 20), "Samuel Kinnett", guiStyle);
            //Change font size for bio
            guiStyle.fontSize = 16;
            //Create bio label. Add guiStyle. 
            GUI.Label(new Rect(10, 100, 60, 20), "Sam's Bio...", guiStyle);
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                aboutPress = true;
                samPress = false;
            }
        }
        //CONOR BUTTON PRESSED (Expansion of AboutPress)
        if (conorPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle.
            GUI.Label(new Rect(220, 30, 60, 20), "Conor Harrison", guiStyle);
            //Change font size for bio
            guiStyle.fontSize = 16;
            //Create bio label. Add guiStyle. 
            GUI.Label(new Rect(10, 100, 60, 20), "Conor's Bio...", guiStyle);
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                aboutPress = true;
                conorPress = false;
            }
        }
        //JAMES BUTTON PRESSED (Expansion of AboutPress)
        if (jamesPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle.
            GUI.Label(new Rect(220, 30, 60, 20), "James Courtney", guiStyle);
            //Change font size for bio
            guiStyle.fontSize = 16;
            //Create bio label. Add guiStyle. 
            GUI.Label(new Rect(10, 100, 60, 20), "James's Bio...", guiStyle);
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                aboutPress = true;
                jamesPress = false;
            }
        }
        //WARWICK BUTTON PRESSED (Expansion of AboutPress)
        if (warwickPress == true)
        {
            //Choose font size, colour and style of text
            guiStyle.fontSize = 32;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Create title label. Add guiStyle.
            GUI.Label(new Rect(220, 30, 60, 20), "Warwick Lynam", guiStyle);
            //Change font size for bio
            guiStyle.fontSize = 16;
            //Create bio label. Add guiStyle. 
            GUI.Label(new Rect(10, 100, 860, 820), "Hi, all! Warwick here.\n\nAs the last ‘founder’ of this game, I came late into the programming stage,\narriving just in time to revamp the menu systems, creating the options menus and the pause menu.\nStill, I’ve enjoyed my time working with my team and am proud that I have contributed to such a marvellous achievement.\n\nI hope you enjoy our game!\n\n Warwick");
            if (GUI.Button(new Rect(400, 250, 100, 50), "Back", guiStyle))
            {
                aboutPress = true;
                warwickPress = false;
            }
        }
        //PAUSE MENU GUI
        if (pauseMenuActive == true)
        {
            //Pause the game
            gameController = obj_GameController.GetComponent<GameController>();
            gameController.Pause();
            //Change the font size, colour and style for the buttons
            guiStyle.fontSize = 16;
            guiStyle.normal.textColor = new Color(80, 80, 80);
            guiStyle.font = myFont;
            //Remove additional text
            canvas.SetActive(false);
            if (GUI.Button(new Rect(340, 50, 200, 50), "Continue Game", guiStyle) || Input.GetKey(KeyCode.Space))
            {
                //Return additional text and resume game
                canvas.SetActive(true);
                pauseMenuActive = false;
            }
            //Create Save button
            if (GUI.Button(new Rect(370, 150, 200, 50), "Save Game", guiStyle))
            {
                //Links to the SaveGame void down below
                SaveGame();
            }
            //Create back button
            if (GUI.Button(new Rect(400, 250, 200, 50), "Quit", guiStyle))
            {
                areYouSure = true;
                pauseMenuActive = false;
            }
        }
        if (areYouSure == true)
        {
            GUI.Label(new Rect(340, 30, 200, 50), "ARE YOU SURE?", guiStyle);
            GUI.Label(new Rect(200, 50, 200, 50), "ALL UNSAVED DATA WILL BE LOST", guiStyle);
            if (GUI.Button(new Rect(200, 250, 100, 50), "Yes", guiStyle))
            {
                areYouSure = false;
                SceneManager.LoadScene("Main Menu");
            }
            if (GUI.Button(new Rect(600, 250, 100, 50), "No", guiStyle))
            {
                areYouSure = false;
                pauseMenuActive = true;
            }
        }
    }

    public void pauseMenu()
    {
        pauseMenuActive = true;

    }

    public void SaveGame()
    {
        //TODO: Put the code for saving the game here!!!
    }
    
}
