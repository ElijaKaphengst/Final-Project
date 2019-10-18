using System.Collections.Generic;
using UnityEngine;

namespace Character.NPC.PathSystem
{
    /// <summary> Controlls the movement for the NPC whose 'WalkType' is set to 'Random'. </summary>
    public class AIMaster : MonoBehaviour
    {
        #region Singlton

        /// <summary> The Intance of this class. Use it to get access to the class. </summary>
        public static AIMaster instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        #endregion

        /// <summary> List all NPCs that move randomly. </summary>
        public List<NpcController> randomNpcs;
        /// <summary> The index of the closest area. </summary>
        private int closestAreaInd;
        /// <summary> Array of all area controlls that are accessable. </summary>
        private RandomAreaControll[] randomAreaControlls;
        /// <summary> Array of the bounds of all areas. </summary>
        private Bounds[] allAreaBounds;

        private void Start()
        {
            GetRandomMoveTypeNpc();
            GetBounds();
        }

        /// <summary> Sets the 'randomNpc' list to all NPCs that should walk randomly. </summary>
        void GetRandomMoveTypeNpc()
        {
            //Find all NpcControllers in the scene.
            NpcController[] npc = FindObjectsOfType<NpcController>();

            //Loops through all found NPCs and if the 'WalkType' of the NPC is 'Random' the add it to the 'randomNpcs' list.
            for (int i = 0; i < npc.Length; i++)
            {
                if (npc[i].walkType == WalkType.Random)
                    randomNpcs.Add(npc[i]);
            }
        }

        /// <summary> Get all 'RandomAreaControll' Objects in the scene and the bounds of them. </summary>
        void GetBounds()
        {
            //Set the 'randomAreaControlls' array
            randomAreaControlls = FindObjectsOfType<RandomAreaControll>();
            //Set the length of the 'allAreaBounds' array.
            allAreaBounds = new Bounds[randomAreaControlls.Length];

            //Loops through 'allAreaBounds'.
            for (int i = 0; i < allAreaBounds.Length; i++)
            {
                //Set the bound to the bound of the 'randomAreaControll'.
                allAreaBounds[i] = randomAreaControlls[i].bounds;
                //Check if the area has an NPC inside of it.
                randomAreaControlls[i].GetControllerInside();
            }
        }

        /// <summary> Finds the closest area of an NPC. </summary>
        /// <param name="npc"> The NPC that want to get it's closest area. </param>
        public void GetNearestArea(NpcController npc)
        {
            //Return if there are for areas.
            if (allAreaBounds.Length <= 0)
                return;

            //If there is just one area set it to this and return then.
            if (allAreaBounds.Length == 1)
            {
                npc.areaControll = randomAreaControlls[0];
                return;
            }

            //Create a Vector3 wich can store the closest posion to the NPC for all areas.
            Vector3[] closestAreaPos = new Vector3[allAreaBounds.Length];

            //Loops through all areas.
            for (int i = 0; i < allAreaBounds.Length; i++)
            {
                //Set the 'closestAreaPos' for each area.
                closestAreaPos[i] = allAreaBounds[i].ClosestPoint(npc.transform.position);

                // Set the 'closestAreaInd' to 0 at the start.
                if (i == 0)
                {
                    closestAreaInd = 0;
                }   //Check if the distance between the current area position and the NPC is smaller than the closest area position and if it's then set the 'closestAreaInd' to i.
                else if (i >= 1 && Vector3.Distance(npc.transform.position, closestAreaPos[i]) < Vector3.Distance(npc.transform.position, closestAreaPos[closestAreaInd]))
                    closestAreaInd = i;
            }
            //Set the area in wich the NPC will radomly move to the closest one.
            npc.areaControll = randomAreaControlls[closestAreaInd];
        }
    }
}
