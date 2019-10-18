using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the loading screen.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    //true if something is loading and the loading screen is enabled
    public bool isLoading;
    //the loading screen Object
    public GameObject loadingScreen;

    /// <summary>
    /// Enables the Loading screen.
    /// </summary>
    public void EnableLoadingScreen()
    {
        //set loading to true
        isLoading = true;
        //set the loading screen to true
        loadingScreen.SetActive(isLoading);
    }

    /// <summary>
    /// Disables the loading screen.
    /// </summary>
    public void DisableLoadingScreen()
    {
        // loading to false
        isLoading = false;

        //Check if the loaded scene is not the base scene and set it then to false
        if (SceneManager.GetActiveScene().buildIndex != 0 || SceneManager.sceneCount > 1)
            loadingScreen.SetActive(isLoading);
    }

}
