using UnityEngine;


/// <summary> Manages evething with UI. Currently just if the cursor is visible. </summary>
public class UiManager : MonoBehaviour
{
    #region Singleton

    /// <summary> The Intance of this class. Use it to get access to the class. </summary>
    public static UiManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    /// <summary> If the cusor will be visible at the start of the game. </summary>
    public bool wantToDisableCursorAtStart;

    private void Start()
    {
        //Sets the cursor mode at the start.
        SetCursorDisplayMode(wantToDisableCursorAtStart);  
    }

    /// <summary>
    /// Set the visibility and lock the Cursor.
    /// </summary>
    /// <param name="showCursor">If the cursor is visible or not.</param>
    public void SetCursorDisplayMode(bool showCursor)
    {
        //Sets the Cursor visibility.
        Cursor.visible = showCursor;

        //Sets the lock mode of the cursor.
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
