using UnityEngine;

namespace Character
{
    /// <summary>
    /// Interface of an character. Implements the speed variable and move method.
    /// </summary>
    public interface IActor
    {
        /// <summary> The speed that need to be implemented by an character class. </summary>
        float Speed { get; set; }

        /// <summary>
        /// The Move Method that need to be implemented by an character class.
        /// </summary>
        /// <param name="targetPosition"> The target position where the character should go. </param>
        void Move(Vector3 targetPosition);
    }
}
