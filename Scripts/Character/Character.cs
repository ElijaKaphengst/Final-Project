using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    /// <summary> Main Class to controll the movement of the characters. </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour, IActor
    {
        /// <summary> The speed of the character, wich is used to to store the main speed. </summary>
        [SerializeField]//Serialized so that the value can be changed in the inspector.
        private float speed = 5f;
        /// <summary> The speed of the character </summary>
        public float Speed
        {
            get { return speed; }
            set
            {
                if (speed < 0)
                    speed = 0;
                else
                    speed = value;
            }
        }

        /// <summary> If the character can move. </summary>
        public bool canMove = true;
        /// <summary> Reference of the NavMeshAgent of the character. </summary>
        [HideInInspector]
        public NavMeshAgent agent;
        /// <summary> The Rigidbody attached to the chraracter. </summary>
        [HideInInspector]
        public Rigidbody rigid;
        /// <summary> The Chracter that will be followed by this character. </summary>
        public Character targetCharacter;

        void OnValidate()
        {
            //Sets the private speed, wich is visible in the inspector, equels to the public speed float.
            Speed = speed;
        }

        /// <summary>
        /// Method to move the character
        /// </summary>
        /// <param name="targetPosition"> The position where the character will be move. </param>
        virtual public void Move(Vector3 targetPosition)
        { }

        /// <summary>
        /// Sets the speed of the character
        /// </summary>
        /// <param name="speed"> How fast the character will move. </param>
        virtual public void SetSpeed(float speed)
        {
            Speed = speed;
        }
    }
}
