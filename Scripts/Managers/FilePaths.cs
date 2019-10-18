using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Stores all Pathes from the files or folder that are needed.
    /// </summary>
    public class FilePaths : MonoBehaviour
    {
        public static string StreamingAssetsPath;
        public const string AssetFolderPath = "Assets";
        public const string GizmosRessourcesPath = "Assets/Gizmos/Resources";
        public const string StreamingAssetsPathProject = "Assets/StreamingAssets";
        public const string RessourcesPath = "Assets/Resources";
        public const string ScriptableFolderPath = "Assets/ScriptableObjects";
        public const string DialoguePath = "/Dialogue.json";

        private void Awake()
        {
            //Set the StreamingAssets path. Has a different path on other divices
            StreamingAssetsPath = Application.streamingAssetsPath;
        }
    }
}
