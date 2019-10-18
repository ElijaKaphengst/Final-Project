using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Managers
{
    /// <summary> Managed the Scenes. Used for loading and unloading. </summary>
    public class SceneLoader : MonoBehaviour
    {
        #region Singleton
        /// <summary> The Intance of this class. Use it to get access to the class. </summary>
        public static SceneLoader instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        #endregion

        /// <summary> The Index of the current scene. </summary>
        private int curSceneInd;
        /// <summary> Reference of the '000_BaseScene' Index wich contains the GameManager and bit more. </summary>
        private int baseSceneInd = 0;
        /// <summary> Reference of the LoadingScreen Class to use it while loading. </summary>
        private LoadingScreen loadingScreen;

        private void Start()
        {
            loadingScreen = GetComponent<LoadingScreen>();

            //Apply the Method that defines what should happen to the 'sceneLoaded' event.
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            //Set the current scene index.
            curSceneInd = SceneManager.GetActiveScene().buildIndex;

            //Check if the current scnene is the baseScene and if it's, load the main menu.
            if (curSceneInd == baseSceneInd)
                LoadSceneToBase(1);
        }

        /// <summary> Load a Scene by an Index. </summary>
        /// <param name="sceneInd"> The Index of the Scene that should be loaded. </param>
        public void LoadSceneToBase(int sceneInd)
        {
            //Enable the loading screen.
            loadingScreen.EnableLoadingScreen();

            //Load the base scene with managers inside.
            SceneManager.LoadScene(baseSceneInd, LoadSceneMode.Single);

            //Add the wanted scene to the base scene.
            SceneManager.LoadScene(sceneInd, LoadSceneMode.Additive);
        }

        /// <summary>
        /// This that should happen when a Scene was loaded. Added to the 'SceneManager.sceneLoaded' event.
        /// </summary>
        /// <param name="loadeScene"> The Scene that was loaded. </param>
        /// <param name="loadSceneMode"> The LoadSceneMode how the scene was loaded. </param>
        private void SceneManager_sceneLoaded(Scene loadeScene, LoadSceneMode loadSceneMode)
        {
            //Deactivate the loading screen when a scene has been loaded.
            loadingScreen.DisableLoadingScreen();

            //Create a new List of ints in wich are later the indexes of all current loaded Scenes stored.
            List<int> loadedScenes = new List<int>();

            //Add the current loaded scenes to the list
            for(int i = 0; i < SceneManager.sceneCount; i++)
                loadedScenes.Add(SceneManager.GetSceneAt(i).buildIndex);

            //Set the cusor visibility on the current loaded scene. On main menu false and ingame true.
            if (loadedScenes.Contains(1))
                UiManager.instance.SetCursorDisplayMode(true);
            else if (loadedScenes.Contains(2))
                UiManager.instance.SetCursorDisplayMode(false);
        }
    }
}
