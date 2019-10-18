using UnityEngine;

namespace Character.Animation
{
    /// <summary> Controlls the Animation of the Player. </summary>
    public class PlayerAnimation : AnimController
    {
        /// <summary> Reference to the 'PlayerController' to get the movement of the player. </summary>
        PlayerController playerController;

        private void Start()
        {
            //Initialize the diffrent classes that are needed.
            character = GetComponentInParent<PlayerController>() as PlayerController;
            playerController = (PlayerController)character;
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            //Check if the player can move and if not then return.
            if (!playerController.canMove)
                return;

            //Store the velocity of the player in a new Vector3 wich contains the horizontal and vertical speed
            var input = Vector3.ClampMagnitude(new Vector3(playerController.curX, 0, playerController.curZ), 1);
            //Animates the player
            Animate(transform.rotation, input, playerController);
        }
    }
}
