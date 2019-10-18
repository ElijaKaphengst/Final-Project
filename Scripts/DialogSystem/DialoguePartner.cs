using UnityEngine;
using Character.NPC;

namespace Character.DialogueSystem
{
    /// <summary> Class wich is attached to the NPCs that are talkable. </summary>
    public class DialoguePartner : MonoBehaviour
    {
        /// <summary> The progress of all dialogues that an NPC can have. </summary>
        public int dialogueProgress;
        /// <summary> All dialogues the NPC have. </summary>
        public NpcDialogue npcDialogue;
        /// <summary> Reference of the NpcClass of the NPC. </summary>
        public NpcClass npcClass;

        private void Start()
        {
            npcClass = GetComponent<NpcClass>();
            //Set the 'npcDialogue'
            npcDialogue = GetNpcDialogue();
        }

        /// <summary>
        /// Searches the NpcDialogue class of the NPC.
        /// </summary>
        /// <returns> Returns the NpcDialogue class of the NPC if he has one. If not return null. </returns>
        NpcDialogue GetNpcDialogue()
        {
            //Loops through all NpcDialogue classes.
            for (int i = 0; i < DialogueManager.instance.Storage.DialogueList.Length; i++)
            {
                //Check if the NpcDialogue class is the one from the NPC
                if (DialogueManager.instance.Storage.DialogueList[i].Name == npcClass.npc.npcName)
                    return DialogueManager.instance.Storage.DialogueList[i];
            }
            return null;
        }
    }
}
