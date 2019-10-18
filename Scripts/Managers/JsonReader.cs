using System.IO;
using UnityEngine;

namespace Managers
{
    /// <summary> Reades and writes jsons. </summary>
    public class JsonReader : MonoBehaviour
    {
        #region Singleton

        /// <summary> The Intance of this class. Use it to get access to the class. </summary>
        public static JsonReader instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else if (instance != this)
                Destroy(gameObject);
        }

        #endregion

        /// <summary> Load data to a class from a Json-file. </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json Path">The path where the Json is stored.</param>
        /// <param name="json Object">The class in wich the Json data should be stored.</param>
        public static void ReadFromJson<T>(string jsonPath, ref T jsonObject)
        {
            //Check if the file exits, if not the return.
            if (!File.Exists(jsonPath))
                return;
            else
            {
                //Read the json to a string.
                string jsonString = File.ReadAllText(jsonPath);
                //Convert that string to the target object.
                jsonObject = JsonUtility.FromJson<T>(jsonString);
            }
        }

        /// <summary> Overwrites a class from a Json-file. </summary>
        /// <param name="json Path">The path where the Json is stored.</param>
        /// <param name="json Object">The class to overwrite.</param>
        public static void OverwriteFromJson(string jsonPath, ref object jsonObject)
        {
            //Check if the file exits, if not the return.
            if (!File.Exists(jsonPath))
                return;
            else
            {
                //Read the json to a string.
                string jsonString = File.ReadAllText(jsonPath);
                //Overwrites the target object.
                JsonUtility.FromJsonOverwrite(jsonString, jsonObject);
            }
        }

        /// <summary> Creates a new Json-file. </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json Path">The path to save the Json.</param>
        /// <param name="json Object">The class to write the Json.</param>
        public static void WriteToJson<T>(string jsonPath, T jsonObject)
        {
            //Create a new string wich represents the json-file.
            string jsonString = JsonUtility.ToJson(jsonObject);

            //using the StreamWriter to write and create a new file at the 'jsonPath'.
            using (StreamWriter file = File.CreateText(jsonPath))
            {
                //write the json inside the created file.
                file.Write(jsonString);
            }
        }
    }
}
