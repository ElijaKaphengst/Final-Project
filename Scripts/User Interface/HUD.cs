using UnityEngine;

/// <summary> Controlls the HUD of the player. </summary>
public class HUD : MonoBehaviour
{
    /// <summary> The GameObject wich will be activated when the player can interact with something. </summary>
    public GameObject interactDisplay;

    /// <summary> Sets the interaction display true if the player is able to interact with something. </summary>
    /// <param name="isInteracting"> Is the player interacting with something? </param>
    /// <param name="interactObject"> The object the player can interact with. </param>
    public void CanInteract(bool isInteracting, Object interactObject = null)
    {
        //Checks if the the player is interacting with something or the interact object is equel to null and it it's set the deactivate the 'interactDisplay'.
        if (interactObject == null || isInteracting == true)
            interactDisplay.SetActive(false);
        else //Otherwise set activate the interact display.
            interactDisplay.SetActive(true);
    }
}
