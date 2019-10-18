using Character.DialogueSystem;
using Character.NPC;
using Managers;
using UnityEngine;

namespace Character
{
    /// <summary> Controlls the interactions of the player. </summary>
    public class PlayerInteractions : CharacterInteractions
    {
        /// <summary> The eyes of the player. </summary>
        public GameObject eyes;

        /// <summary> Reference of the HUD class to controll the interaction display. </summary>
        private HUD hud;
        /// <summary> Reference of the movement controller of the player. </summary>
        private PlayerController playerController;

        private void Start()
        {
            hud = GetComponent<HUD>();
            playerController = GetComponent<PlayerController>();
            hitObjectRef = GameObject.FindGameObjectWithTag(Tags.BaseObject.ToString());
        }

        private void Update()
        {
            //Check if the hitted object is not equel to its reference or null and if it is the call the Interact method, otherwise set the interact display in the hud to false.
            if (hitObject != hitObjectRef && hitObject != null)
                Interact();
            else
                hud.CanInteract(isInteracting);
        }

        private void FixedUpdate()
        {
            //Call this method in fixedUpdate because it calls physics.
            CheckInteractionRange();
        }

        public override void CheckInteractionRange()
        {
            //Set the start position of the CapsuleCastAll.
            base.CheckInteractionRange();
            //Set the end position of the CapsuleCastAll.
            capsuleEndPoint = eyes.transform.position + transform.forward * capsuleOffset;
            //Set the hitted object and call the CapsuleCastAll.
            hitObject = ObjectInInteractionRange(capsuleStartPoint, capsuleEndPoint, transform, hitObjectRef.GetType(), hitTag.ToString()) as GameObject;
        }

        void Interact()
        {
            //Create a new GameObject and set it equel to the hitted object.
            var hitObj = hitObject as GameObject;

            //Check if the hitted GameObject is an NPC.
            if (hitObj.tag == Tags.NPC.ToString())
            {
                //Get the 'NpcClass' of the NPC.
                var npc = hitObj.GetComponent<NpcClass>();

                //Check if the NPC hast the 'NpcClass'.
                if (npc != null)
                {
                    //Set the interact display in the hud true.
                    hud.CanInteract(isInteracting, hitObj);

                    //Check if the NPC has the player in his interaction range and if he is able to talk and if the player isn't interacting.
                    if (npc.interactions.playerInRange && npc.canTalk && !isInteracting)
                    {
                        //Check if the interact Key is pressed and if it's then start a dialogue with the NPC.
                        if (Input.GetKeyDown(KeyCode.F))
                            DialogueManager.instance.StartDialogue(npc.npc, playerController, this, npc.dialoguePartner);
                    }
                }
            }
        }
    }
}
