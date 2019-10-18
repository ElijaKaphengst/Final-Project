using UnityEngine;
using Managers;

namespace Character.Animation
{
    /// <summary> Main Controller for the Animations </summary>
    public class AnimController : MonoBehaviour
    {
        /// <summary> The time that the vertical animations need to get in target speed. </summary>
        public float speedDampTime = .1f;
        /// <summary> The time that the horizontal animations need to get in target speed. </summary>
        public float angularSpeedDampTime = .7f;
        /// <summary> Reference of the Animator. </summary>
        [HideInInspector]
        public Animator anim;
        /// <summary> Reference of the target character that will be animated. </summary>
        [HideInInspector]
        public Character character;
        /// <summary> The speed of the animation. Vector3 because it uses an blend tree and wamt to use Quaternions as well.  </summary>
        private Vector3 localVelocity;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        /// <summary> Animates the Chracter </summary>
        /// <param name="localRotation"> The rotation of the animated GameObject. </param>
        /// <param name="inputVelocity"> How fast the Chararcter move in different directions. </param>
        /// <param name="animCharacter"> The Character that will be animated. </param>
        virtual public void Animate(Quaternion localRotation, Vector3 inputVelocity, Character animCharacter)
        {
            //Create a new Vector3 and store the character speed in it.
            var velocity = new Vector3(inputVelocity.x * animCharacter.Speed, 0, inputVelocity.z * animCharacter.Speed);
            localVelocity = Quaternion.Inverse(localRotation) * (velocity / animCharacter.Speed);

            //Sets the speed float of the animator
            anim.SetFloat(HashIDs.c_SpeedFloat, localVelocity.z, speedDampTime, Time.deltaTime);
            anim.SetFloat(HashIDs.c_AngularSpeedFloat, localVelocity.x, angularSpeedDampTime, Time.deltaTime);
        }

        private void LateUpdate()
        {
            //Sets the local position and rotation to zero. Otherwise it will move the charcter to fast and not right.
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}
