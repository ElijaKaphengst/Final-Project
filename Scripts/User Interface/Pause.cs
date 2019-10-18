using UnityEngine;
using Managers;

/// <summary> Used for pause the game and the the pausemenu. </summary>
public class Pause : MonoBehaviour
{
    /// <summary> The pause menu GameObject. </summary>
    public GameObject pauseMenu;
    /// <summary> If the game is paused </summary>
    public bool isPaused = false;

    private void Update()
    {
        //Check if the Pause Key was pressed and if it was the call the 'PauseGame' method.
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
    }

    /// <summary> Pauses or unpauses the game. </summary>
    public void PauseGame()
    {
        //Change the state of the 'isPaused' bool.. 
        isPaused = !isPaused;
        //Set the active of the pause menu.
        pauseMenu.SetActive(isPaused);
        //Set the cursor visibility.
        UiManager.instance.SetCursorDisplayMode(isPaused);
        //Set the timeScale of the game to 0 if 'isPaused' is true, otherwise set it to 1.
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary> Exits the game level and loads the main menu. </summary>
    public void ExitGame()
    {
        //Loads the main menu.
        SceneLoader.instance.LoadSceneToBase(1);
        //unpause the game so the timeScale is right.
        PauseGame();
    }
}
