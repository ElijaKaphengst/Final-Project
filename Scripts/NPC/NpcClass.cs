using UnityEngine;
using Character.DialogueSystem;

namespace Character.NPC
{
    /// <summary> Main component of a NPC. Used to get the different controllers and checking main thing line if the NPC is talkable. </summary>
    [RequireComponent(typeof(NpcController), typeof(NpcInteractions))]
    public class NpcClass : MonoBehaviour
    {
        /// <summary> The ScriptableNpc that represents the NPC. </summary>
        public ScriptableNpc npc;
        /// <summary> True if the player can start a dialogue with the NPC. </summary>
        public bool canTalk;

        /// <summary> Reference of the NpcController class, wich controlls the movement of the NPC. </summary>
        private NpcController controller;
        /// <summary> Reference of the NpcInteractions class, wich controlls the interactions of the NPC. </summary>
        [HideInInspector]
        public NpcInteractions interactions;
        /// <summary> Reference of the DialogueParner class, wich is added wheh the player is able to have a dialogue with the NPC. </summary>
        [HideInInspector]
        public DialoguePartner dialoguePartner;

        private void Start()
        {
            CheckNpc();
            controller = GetComponent<NpcController>();
            interactions = GetComponent<NpcInteractions>();
        }

        /// <summary> Checks if the NPC is someone who is able hace dialogues. </summary>
        void CheckNpc()
        {
            if (npc == null)
                return;

            if (npc is SpecialScriptableNpc)
            {
                //Add the DialoguePartner component.
                dialoguePartner = gameObject.AddComponent<DialoguePartner>();
                canTalk = true;
            }
            else
            {
                //Check if there is a DialoguePartner component and if it's the destroy it.
                if (GetComponent<DialoguePartner>())
                    Destroy(GetComponent<DialoguePartner>());
                canTalk = false;
            }
        }

        /// <summary>
        /// Sets how the NPC is moving.
        /// </summary>
        /// <param name="walkType"> The walkType how the NPC should move.
        /// </param>
        public void SetMoveType(WalkType walkType)
        {
            controller.walkType = walkType;
        }
    }
}
