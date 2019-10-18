using UnityEngine;

namespace Character.NPC.PathSystem
{
    [System.Serializable]
    public class Path : MonoBehaviour
    {
        /// <summary> Array that stores all waypoints of the path. </summary>
        [HideInInspector]
        public Vector3[] WayPoints;
        /// <summary> How the Path can be walked. </summary>
        [HideInInspector]
        public PathType pathType;
        /// <summary> The Tool that is used in the Editor scene view. </summary>
        [HideInInspector]
        public WayPointsMoveTool moveTool;
        /// <summary> If the waypoints should be connected with the main transform of the GameObject in the scene view. </summary>
        [HideInInspector]
        public bool connectWithObject;
        /// <summary> Whether flags should be displayed at the waypoints in the scene view. </summary>
        [HideInInspector]
        public bool showFlag;
        /// <summary> If the waypoints should be connected with each other in the scene view. </summary>
        [HideInInspector]
        public bool showPath;
        /// <summary> If the Navigation mesh should be shown in the scene view. </summary>
        [HideInInspector]
        public bool showNavMesh;
        /// <summary> The layers that will be hit by the button wich snaps the waypoints to the ground. </summary>
        [HideInInspector]
        public LayerMask layerMask;

        /// <summary>
        /// Returns the next waypoint position and set the index of the target waypoint.
        /// </summary>
        /// <param name="waypointInd"> The previous target waypoint index. </param>
        /// <returns> Returns a Vector3 of the position from the next waypoint </returns>
        public Vector3 WalkAlongPath(ref int waypointInd)
        {
            //Check the 'pathTpe' from the path.
            switch (pathType)
            {
                case PathType.Loop:
                    //Set the 'waypointInd' to the next waypoint in the 'WayPoints' array.
                    waypointInd = (waypointInd + 1) % WayPoints.Length;
                    //Returns the waypoint at the 'waypointInd'.
                    return WayPoints[waypointInd];
                case PathType.Random:
                    //Returns a random waypoint.
                    return WayPoints[Random.Range(0, WayPoints.Length)];
                default:
                    return WayPoints[0];
            }
        }
    }

    /// <summary> How the path will be used. </summary>
    public enum PathType
    {
        /// <summary> Let the NPC walk looping around the path. </summary>
        Loop,
        /// <summary> Let the NPC walk random on the path. </summary>
        Random
    }

    /// <summary> The movetool that is used for editing the waypoints. </summary>
    public enum WayPointsMoveTool
    {
        /// <summary> No movetool selected. </summary>
        None,
        /// <summary> Standard movetool from Unity. </summary>
        MoveTool,
        /// <summary> A rectangle movetool wich moves along the view axis. </summary>
        RectanglHandle,
        /// <summary> A circle movetool wich moves along the view axis. </summary>
        CircleHandle
    }
}
