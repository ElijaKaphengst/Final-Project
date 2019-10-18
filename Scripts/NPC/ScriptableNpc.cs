using UnityEngine;

namespace Character.NPC
{
    /// <summary> Stores all information of an default NPC. </summary>
    [CreateAssetMenu(fileName = "default NPC", menuName = "NPC/Default NPC")]
    public class ScriptableNpc : ScriptableObject
    {
        /// <summary> The name of the NPC. </summary>
        public string npcName;
        /// <summary> The health points of the NPC. </summary>
        public int health = 100;
    }
}
