using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
    /// <summary> Contains Methods to start or close the game. </summary>
    public class MainMenuManager : MonoBehaviour
    {
        /// <summary> Starts the Game. </summary>
        public void StartGame()
        {
            //Load the ingame scene
           SceneLoader.instance.LoadSceneToBase(2);
        }

        /// <summary> Exits the game. </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            //If the user is currently in the Unity Editor the set the playmode to false
            EditorApplication.isPlaying = false;
#else
            //Close the Application if it's uses an Build 
            Application.Quit();
#endif
        }
    }
}
