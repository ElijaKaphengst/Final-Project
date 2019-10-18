using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Stores all Tags.
    /// </summary>
    public enum Tags
    {
        Untagged,
        Respawn,
        Finish,
        EditorOnly,
        MainCamera,
        Player,
        GameController,
        NPC,
        BaseObject,
        Head
    }

    /// <summary>
    /// Stores HashIDs to find the right stuff in the Animator like variables.
    /// </summary>
    public class HashIDs : MonoBehaviour
    {
        /// <summary> Hash for the Locomotion. </summary>
        public static int c_LocomotionState;
        /// <summary> Hash for the speed propert. </summary>
        public static int c_SpeedFloat;
        /// <summary> Hash for the angular speed propert. </summary>
        public static int c_AngularSpeedFloat;

        private void Awake()
        {
            //Find the fight property in the Animator and convert it to an hash
            c_LocomotionState = Animator.StringToHash("Base Layer.Locomotion");
            c_SpeedFloat = Animator.StringToHash("Speed");
            c_AngularSpeedFloat = Animator.StringToHash("Angular Speed");
        }
    }
}
