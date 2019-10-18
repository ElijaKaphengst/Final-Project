using UnityEngine;
using Character.NPC;

namespace Character.Animation
{
    /// <summary> Controlls the Animation of the NPC. </summary>
    public class NpcAnimation : AnimController
    {
        /// <summary> Reference of the 'NpcController' of the NPC to get the movement of the NPC. </summary>
        NpcController npcController;

        private void Start()
        {
            //Initialize the diffrent classes that are needed.
            character = GetComponentInParent<NpcController>();
            npcController = (NpcController)character;
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            //Animate the NPC.
            Animate(transform.rotation, npcController.agent.velocity, npcController);
        }
    }
}
