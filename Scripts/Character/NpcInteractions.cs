using Managers;
using UnityEngine;

namespace Character.NPC
{
    /// <summary> Controls the interactions of an NPC. </summary>
    public class NpcInteractions : CharacterInteractions
    {
        /// <summary> Reference of the movement controller of the NPC. </summary>
        [HideInInspector]
        public NpcController npcController;
        /// <summary> True if the player is in the interaction range. </summary>
        public bool playerInRange;

        private void Start()
        {
            npcController = GetComponent<NpcController>();
            hitObjectRef = GameObject.FindGameObjectWithTag(Tags.BaseObject.ToString());
        }

        private void FixedUpdate()
        {
            CheckInteractionRange();
        }

        /// <summary> Check the interactions Range of the NPC. </summary>
        public override void CheckInteractionRange()
        {
            //Create the start point of the CapsuleCastAll.
            base.CheckInteractionRange();
            //Create the end position of the CapsuleCastAll.
            capsuleEndPoint = capsuleStartPoint + transform.up * npcController.agent.height;
            //Cast the CapsuleCastAll and set it as GameObject to a new variable.
            var objRef = (GameObject)ObjectInInteractionRange(capsuleStartPoint, capsuleEndPoint, transform, hitObjectRef.GetType(), hitTag.ToString());
            //Set the hitObject equel to the hitted object.
            hitObject = objRef;

            //Check if the tag of the hitted object is equel to the target tag and if it's set 'playerInRange' to true, if it isn't set it to false.
            if (objRef == null || objRef.tag != hitTag.ToString())
                playerInRange = false;
            else
                playerInRange = true;
        }
    }
}
