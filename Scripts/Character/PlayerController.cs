using Cinemachine;
using Managers;
using UnityEngine;

namespace Character
{
    /// <summary> Controlls the movement of the player. </summary>
    public class PlayerController : Character
    {
        /// <summary> The Noisesettings for the top rig of the camera. </summary>
        public NoiseSettings topRig;
        /// <summary> The Noisrsettings for the middle rig of the camera. </summary>
        public NoiseSettings middleRig;
        /// <summary> The Noisesettings for the bottom rig of the camera. </summary>
        public NoiseSettings bottomRig;
        /// <summary> The current value of the horizontal input. </summary>
        [HideInInspector]
        public float curX;
        /// <summary> The current value of the vertivcal input. </summary>
        [HideInInspector]
        public float curZ;
        /// <summary> Reference to the CameraController. </summary>
        public CameraController camControll;

        /// <summary> Reference of the Transform from the camera. </summary>
        private Transform camTransform;
        /// <summary> The direction where the player is going. </summary>
        private Vector3 playerDir;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            camTransform = GameObject.FindGameObjectWithTag(Tags.MainCamera.ToString()).transform;
        }

        private void Update()
        {
            //Set the curX and curZ values to the input axis.
            curX = Input.GetAxisRaw("Horizontal");
            curZ = Input.GetAxisRaw("Vertical");

            //Check if the input values are not 0 and if the player is able to move.
            if (curX != 0 && canMove || curZ != 0 && canMove)
            {
                //Set the noise of the camera to 0.
                camControll.SetNoise();
                //Calculate the movement of the player.
                CalculateMovement();
            }
            else
            {
                //Set the noise of the camera to the 3 diffrent noisesetting for each rig.
                camControll.SetNoise(topRig, middleRig, bottomRig);
            }
        }

        private void FixedUpdate()
        {
            //Check if the input values are changing.
            if (curX != 0 || curZ != 0)
            {
                //Move the player to the player direction.
                Move(playerDir);
            }
        }

        /// <summary> Calculates the Movement of the 'playerDir' Vector3.  </summary>
        void CalculateMovement()
        {
            //Check if the player is able to move. If not then return.
            if (!canMove)
                return;

            //Creates a Vector3 and set it to the right direction of the camera transform and multiply it with the current X axis value.
            Vector3 movHorizontal = camTransform.right * curX;
            //Creates a Vector3 and set it to the forward direction of the camera transform and multiply it with the current Z axis value.
            Vector3 movVertical = camTransform.forward * curZ;
            //Add the horizontal movement to the vertical movement.
            Vector3 _velocity = (movHorizontal + movVertical);
            //Set the player direction to the '_velocity' Vector3 above and clamp the X and Z Axis to the absolute value of the axis
            playerDir = new Vector3(Mathf.Clamp(_velocity.x, -Mathf.Abs(_velocity.normalized.x), Mathf.Abs(_velocity.normalized.x)), 0, Mathf.Clamp(_velocity.z, -Mathf.Abs(_velocity.normalized.z), Mathf.Abs(_velocity.normalized.z)));

            //Check if the player is moving.
            if (playerDir != Vector3.zero)
            {
                //Creates a Vector3 and set it to the direction wich the player will move
                Vector3 lookDir = Vector3.RotateTowards(transform.forward, playerDir, Speed * Time.deltaTime, 0f);
                //Set the rotation of the player to the rotation in wich the player will move
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        /// <summary>
        /// Moves the player.
        /// </summary>
        /// <param name="targetPos"> The target position where the player should move. </param>
        public override void Move(Vector3 targetPos)
        {
            //Check if the player can move.
            if (canMove)
            {
                //Moves the rigidbody of the player to target Position
                rigid.MovePosition(transform.position + targetPos * Speed * Time.fixedDeltaTime);
            }
        }
    }
}
