using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC.PathSystem
{
    /// <summary> Controlls a random area. </summary>
    [System.Serializable]
    public class RandomAreaControll : MonoBehaviour
    {
        /// <summary> The bounds of the areas box. </summary>
        public Bounds bounds;

        /// <summary> Get all NPCs that are inside of this area. </summary>
        public void GetControllerInside()
        {
            //Loops through all NPCs that should move random.
            for (int i = 0; i < AIMaster.instance.randomNpcs.Count; i++)
            {
                //Check if the NPC is inside this area.
                if (bounds.Contains(AIMaster.instance.randomNpcs[i].transform.position))
                {
                    //Set the 'hasArea' bool of the NPC to true and the 'areaControll' of the NPC to this so the NPC knows that he is now in an area.
                    AIMaster.instance.randomNpcs[i].hasArea = true;
                    AIMaster.instance.randomNpcs[i].areaControll = this;
                }
            }
        }

        /// <summary> Finds a random position on the NavMesh inside of the area. </summary>
        /// <param name="controller"> The NPC that searches for the position. </param>
        /// <returns> Retuns the Vector3 wich stores an position on the NavMesh. </returns>
        public Vector3 FindRandomPos(NpcController controller)
        {
            //Set the values for each axis inside the bounds. 
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            //Create a Vector3 wich stores the random values.
            Vector3 ranPos = new Vector3(x, y, z);

            //Finds the closest point on the NavMesh.
            if (NavMesh.SamplePosition(ranPos, out NavMeshHit hit, bounds.size.y, controller.agent.areaMask))
            {
                //Check if the hitted point is inside of the area and it is then return the position.
                if (bounds.Contains(hit.position))
                    return hit.position;
            }
            //If the area doesn't contain the hitted position return the position of the NPC so he can search a new one.
            return controller.transform.position;
        }

        /// <summary> Checks if a transform is inside of the area. </summary>
        /// <param name="charPos"> The transform that get checked if it's inside of the area. </param>
        /// <returns> Returns true it the transform is insise of the area. </returns>
        public bool IsInside(Transform charPos)
        {
            //Check if the bounds contains the 'charPos'. If not return false.
            if (bounds.Contains(charPos.position))
                return true;
            return false;
        }
    }
}
