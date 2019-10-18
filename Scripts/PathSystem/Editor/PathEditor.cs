using Managers;
using UnityEditor;
using UnityEditor.AI;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC.PathSystem
{
    /// <summary> Class to edit the path inside of the Editor. </summary>
    [CustomEditor(typeof(Path))]
    public class PathEditor : Editor
    {
        #region Variables/OnEnable

        /// <summary> The path that should be edited. </summary>
        Path path;
        /// <summary> The width of the lines that connect the waypoints. </summary>
        float polyLineWidth = 5f;
        /// <summary> True if the waypoints are visible inside of the inspector. </summary>
        bool showWaypointList = true;
        /// <summary> True if the waypoints should move with the GameObject where the path is attached to. </summary>
        bool moveWithObject = false;
        /// <summary> The last position of the GameObject where the path is attached to. </summary>
        Vector3 lastPos;
        /// <summary> The Layers that should be hit by the snap to ground button. </summary>
        LayerMask layerMask;

        /// <summary> The path reference. Is Serialized for saving and changing them. </summary>
        SerializedObject pathObject;
        /// <summary> Reference of the 'conntectWithObject' bool of the path, wich checks if the waypoints are conntected with the GameObject where the path is attached to. </summary>
        SerializedProperty s_connectWithObjectBool;
        /// <summary> Reference of the 'showFlag' bool of the path, wich checks if the flags for the waypoints should be shown. </summary>
        SerializedProperty s_showFlagBool;
        /// <summary> Reference of the 'showPath' bool of the path, wich check if the path should shown. </summary>
        SerializedProperty s_showPathBool;
        /// <summary> Reference of the 'showNavMesh' bool of the path, wich check if the NavMesh should be shown. </summary>
        SerializedProperty s_showNavMeshBool;
        /// <summary> Reference of the 'WayPoints' array of the path, wich contains all waypoints. </summary>
        SerializedProperty s_WayPointsArray;
        /// <summary> Reference of the 'pathType' enum of the path, wich says how the path should be used. </summary>
        SerializedProperty s_pathTypeEnum;
        /// <summary> Reference of the 'moveTool' enum of the path, wich says how the waypoints can be edited. </summary>
        SerializedProperty s_moveToolEnum;
        
        private void OnEnable()
        {
            if (target == null)
                return;

            //Initialize the path.
            path = (Path)target;
            pathObject = new SerializedObject(path);

            //Initialize the serialized properties.
            s_connectWithObjectBool = serializedObject.FindProperty("connectWithObject");
            s_showFlagBool = serializedObject.FindProperty("showFlag");
            s_showPathBool = serializedObject.FindProperty("showPath");
            s_showNavMeshBool = serializedObject.FindProperty("showNavMesh");

            s_WayPointsArray = serializedObject.FindProperty("WayPoints");
            s_pathTypeEnum = serializedObject.FindProperty("pathType");
            s_moveToolEnum = serializedObject.FindProperty("moveTool");

            //Set the layer mask and the position of the GameObject.
            layerMask = path.layerMask;
            lastPos = path.transform.position;
        }

        private void OnDisable()
        {
            //set 'moveWithObject' to false so it won't will move when reselecting the GameObject.
            moveWithObject = false;
            
            //Save all serialized properties.
            serializedObject.FindProperty("connectWithObject").boolValue = s_connectWithObjectBool.boolValue;
            serializedObject.FindProperty("showFlag").boolValue = s_showFlagBool.boolValue;
            serializedObject.FindProperty("showPath").boolValue = s_showPathBool.boolValue;
            serializedObject.FindProperty("showNavMesh").boolValue = s_showNavMeshBool.boolValue;

            serializedObject.FindProperty("WayPoints").arraySize = s_WayPointsArray.arraySize;
            for (int i = 0; i < s_WayPointsArray.arraySize; i++)
                serializedObject.FindProperty("WayPoints").GetArrayElementAtIndex(i).vector3Value = s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value;

            serializedObject.FindProperty("pathType").enumValueIndex = s_pathTypeEnum.enumValueIndex;
            serializedObject.FindProperty("moveTool").enumValueIndex = s_moveToolEnum.enumValueIndex;
        }

        #endregion

        #region InspectorGUI

        public override void OnInspectorGUI()
        {
            //Update the Path
            serializedObject.Update();
            //Begin the ChangeCheck so if the path was edited it can be redo
            EditorGUI.BeginChangeCheck();

            //Display all things that are needed inside of the Inspector.
            DisplayWaypointList();
            DisplaySceneTools();
            DisplaySnapButtons();

            //Check if something was changed inside of the editor.
            if(EditorGUI.EndChangeCheck())
            {
                //Records an Undo so that you can make to change undone.
                Undo.RecordObject(path, "Path Edited");

                //Save all serialized properties.
                serializedObject.FindProperty("connectWithObject").boolValue = s_connectWithObjectBool.boolValue;
                serializedObject.FindProperty("showFlag").boolValue = s_showFlagBool.boolValue;
                serializedObject.FindProperty("showPath").boolValue = s_showPathBool.boolValue;
                serializedObject.FindProperty("showNavMesh").boolValue = s_showNavMeshBool.boolValue;

                serializedObject.FindProperty("WayPoints").arraySize = s_WayPointsArray.arraySize;
                for (int i = 0; i < s_WayPointsArray.arraySize; i++)
                    serializedObject.FindProperty("WayPoints").GetArrayElementAtIndex(i).vector3Value = s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value;

                serializedObject.FindProperty("pathType").enumValueIndex = s_pathTypeEnum.enumValueIndex;
                serializedObject.FindProperty("moveTool").enumValueIndex = s_moveToolEnum.enumValueIndex;
            }
            //Apply the changed properties to the path.
            serializedObject.ApplyModifiedProperties();
        }

        #region DisplayWaypointList

        /// <summary> Display a List wich contains all waypoints and to buttons to add and remove waypoints. </summary>
        void DisplayWaypointList()
        {
            //Start the horizontal so the buttons are on the same line.
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            //Check if the button for adding a waypoints was pressed.
            if (GUILayout.Button("Add Waypoint", GUILayout.MinHeight(25), GUILayout.MinWidth(100)))
            {
                //Increase the size of the waypoints array.
                s_WayPointsArray.arraySize += 1;

                //Check if the waypoint was the first one added to the array and if it was the set its position near to the position of the path.
                if (s_WayPointsArray.arraySize <= 1)
                    s_WayPointsArray.GetArrayElementAtIndex(s_WayPointsArray.arraySize - 1).vector3Value = new Vector3(path.transform.position.x + 1, path.transform.position.y, path.transform.position.z + 1);
                else //If it wasn't the first one the set the position of the waypoint near to the previous waypoint.
                {
                    s_WayPointsArray.GetArrayElementAtIndex(s_WayPointsArray.arraySize - 1).vector3Value = new Vector3(
                        s_WayPointsArray.GetArrayElementAtIndex(s_WayPointsArray.arraySize - 1).vector3Value.x + 1,
                        s_WayPointsArray.GetArrayElementAtIndex(s_WayPointsArray.arraySize - 1).vector3Value.y,
                        s_WayPointsArray.GetArrayElementAtIndex(s_WayPointsArray.arraySize - 1).vector3Value.z + 1);
                }

            }//Button for removing a waypoint
            else if (GUILayout.Button("Remove Waypoint", GUILayout.MinHeight(25), GUILayout.MinWidth(100)) && s_WayPointsArray.arraySize > 0)
                s_WayPointsArray.arraySize -= 1;
          
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //Creates a foldout in wich the waypoints will be shown.
            showWaypointList = EditorGUILayout.BeginFoldoutHeaderGroup(showWaypointList, "WayPoints | Size: " + s_WayPointsArray.arraySize);

            //Check if the foldout is true and if there are waypoints to show.
            if(showWaypointList && s_WayPointsArray.arraySize > 0)
            {
                //Loops through all waypoints to show them inside of the inspector.
                for (int i = 0; i < s_WayPointsArray.arraySize; i++)
                    s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value = EditorGUILayout.Vector3Field("Waypoint " + i, s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value);
            }

            EditorGUILayout.Space();
        }

        #endregion

        #region DisplayTools

        /// <summary> Creates buttons and popup so select wich tool will be used and how the path should be drawn. </summary>
        void DisplaySceneTools()
        {
            //Create popups for the path typpe and the move tool that are used.
            s_pathTypeEnum.enumValueIndex = (int)(PathType)EditorGUILayout.EnumPopup("Path Type", (PathType)s_pathTypeEnum.enumValueIndex);
            s_moveToolEnum.enumValueIndex = (int)(WayPointsMoveTool)EditorGUILayout.EnumPopup("Waypoints Move Tool", (WayPointsMoveTool)s_moveToolEnum.enumValueIndex);

            EditorGUILayout.Space();

            //Get the width of the inspector window.
            float width = EditorGUIUtility.currentViewWidth;

            //Check if the width of the inspector window is big enough to show all toggle buttons in a row.
            if (width > 400)
            {
                //Create toggle buttons for each bool that sets how the path should be shown.
                EditorGUILayout.BeginHorizontal();
                s_connectWithObjectBool.boolValue = GUILayout.Toggle(s_connectWithObjectBool.boolValue, "Connect with Object", "Button");
                s_showFlagBool.boolValue = GUILayout.Toggle(s_showFlagBool.boolValue, "Show Flag", "Button");
                s_showPathBool.boolValue = GUILayout.Toggle(s_showPathBool.boolValue, "Show Path", "Button");
                s_showNavMeshBool.boolValue = GUILayout.Toggle(s_showNavMeshBool.boolValue, "Show NavMesh", "Button");

                //Check if the NavMesh should be shown.
                if (s_showNavMeshBool.boolValue)
                    NavMeshVisualizationSettings.showNavigation = 1;
                else
                    NavMeshVisualizationSettings.showNavigation = 0;

                EditorGUILayout.EndHorizontal();
            }
            else //Creates the toggle in two rows so there have enough space to be visible
            {
                //Create toggle buttons for each bool that sets how the path should be shown.
                EditorGUILayout.BeginHorizontal();
                s_connectWithObjectBool.boolValue = GUILayout.Toggle(s_connectWithObjectBool.boolValue, "Connect with Object", "Button");
                s_showFlagBool.boolValue = GUILayout.Toggle(s_showFlagBool.boolValue, "Show Flag", "Button");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                s_showPathBool.boolValue = GUILayout.Toggle(s_showPathBool.boolValue, "Show Path", "Button");
                s_showNavMeshBool.boolValue = GUILayout.Toggle(s_showNavMeshBool.boolValue, "Show NavMesh", "Button");

                //Check if the NavMesh should be shown.
                if (s_showNavMeshBool.boolValue)
                    NavMeshVisualizationSettings.showNavigation = 1;
                else
                    NavMeshVisualizationSettings.showNavigation = 0;

                EditorGUILayout.EndHorizontal();
            }
        }

        #endregion

        #region SnapButtons

        /// <summary> Lets the buttons for the snaping draw in the inspector. </summary>
        void DisplaySnapButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            //Button for snaping all waypoints to the ground.
            if (GUILayout.Button("Snap To Ground"))
            {
                //Check for each waypoint if if the waypoint is above of a GameObject and if it is set the position to the hitted position.
                for(int i = 0; i < s_WayPointsArray.arraySize; i++)
                {
                    if (Physics.Raycast(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, Vector3.down, out RaycastHit hit, 10f, layerMask))
                        s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value = new Vector3(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value.x, hit.point.y + .1f, s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value.z);
                }
            }

            //Button for snaping all waypoints to the Navigation Mesh.
            if (GUILayout.Button("Snap To NavMesh"))
            {
                //Checks everey waypoint if it is near to the NavMesh and if it's then set the waypoint to the hitted position. 
                for(int i = 0; i < s_WayPointsArray.arraySize; i++)
                {
                    if(NavMesh.SamplePosition(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, out NavMeshHit hit, 10, NavMesh.AllAreas))
                        s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value = hit.position + new Vector3(0, .1f);
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //Shows the Layer mask that should be hit by the Raycast for snaping to ground.
            LayerMask tempMaskLayer = EditorGUILayout.MaskField("Ground Hit Layers",InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask), InternalEditorUtility.layers);
            layerMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMaskLayer);

            EditorGUILayout.Space();

            //Create a toggel button for if the path should be moved with the GameObject.
            moveWithObject = GUILayout.Toggle(moveWithObject, "Move path with the Object", "Button");
        }

        #endregion

        #endregion

        #region SceneGUI

        private void OnSceneGUI()
        {
            //Check if there are waypoints to look at in the Scene.
            if (s_WayPointsArray.arraySize <= 0)
                return;

            //Update the path.
            pathObject.Update();
            //Begin the ChangeCheck to save the canges made to the path
            EditorGUI.BeginChangeCheck();

            //Check if the path should move with the path and move it with it by calling the 'SnapToParentObj' method.
            if (moveWithObject)
                SnapToParentObj();

            //Check if the path should be shown in the inspector and if it does then call the 'ConnectWaypointsWithPolyLine' method and the 'DrawWayPointIndex' method to also show the index of each waypoint next to it.
            if (s_showPathBool.boolValue)
            {
                ConnectWaypointsWithPolyLine();
                DrawWayPointIndex();
            }
            //Check if a move tool for the waypoints is selected.
            if (s_moveToolEnum.enumValueIndex != (int)WayPointsMoveTool.None)
                DrawTransformHandles();
            //Check if the waypoints should be conntected wich the GameObject where the path is attached to.
            if (s_connectWithObjectBool.boolValue)
                ConnectMainTransformWithWaypoints();

            //Check if the path was edited inside of the scene view
            if (EditorGUI.EndChangeCheck())
            {
                //Records an Undo so that the user can redo the change.
                Undo.RecordObject(path, "Path Edited");

                //saves the position of the waypoints.
                for (int i = 0; i < s_WayPointsArray.arraySize; i++)
                    path.WayPoints[i] = s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value;
            }
            //Apply the edited properties to the path.
            pathObject.ApplyModifiedProperties();
        }

        #region DisplaySceneTools

        /// <summary> Displays a polyline between each waypoint </summary>
        void ConnectWaypointsWithPolyLine()
        {
            //Sets the Color of the Polyline.
            Handles.color = Color.yellow;

            //Creates a Vector3 array to the length of the waypoints that is used for drawing the polyline.
            Vector3[] vectors = new Vector3[s_WayPointsArray.arraySize];

            //Sets every positions of the Vector3 array to the position of the related waypoint.
            for (int i = 0; i < s_WayPointsArray.arraySize; i++)
                vectors[i] = s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value;

            //Draws the polyline for all waypoints.
            Handles.DrawAAPolyLine(polyLineWidth, vectors);

            //Checks how the path should be used and connects the last and the first Waypoint if it's 'Loop'.
            if (s_pathTypeEnum.enumValueIndex == (int)PathType.Loop)
                Handles.DrawAAPolyLine(polyLineWidth, s_WayPointsArray.GetArrayElementAtIndex(s_WayPointsArray.arraySize - 1).vector3Value, s_WayPointsArray.GetArrayElementAtIndex(0).vector3Value);

        }

        /// <summary> Displays a tool for every waypoint wich can edit the position of the waypoints </summary>
        void DrawTransformHandles()
        {
            //Loops through all waypoints.
            for (int i = 0; i < s_WayPointsArray.arraySize; i++)
            {
                //Creates the Snap value so when the button for snapping is pressed, snap it every 0.5m.
                Vector3 snap = Vector3.one * .5f;
                //Creates the target position where the waypoint should be moved to.
                Vector3 targetPos = s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value;

                //Check wich move tool should be used.
                switch ((WayPointsMoveTool)s_moveToolEnum.enumValueIndex)
                {
                    case WayPointsMoveTool.MoveTool:
                        //Creates a positionhandle like the normal move tool and set the target position to this handle. 
                        targetPos = Handles.PositionHandle(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, Quaternion.identity);
                        break;
                    case WayPointsMoveTool.RectanglHandle:
                        //Creates a positionhandle in a rectangle shape and set the target position to this handle.
                        targetPos = Handles.FreeMoveHandle(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, Quaternion.identity, 1, snap, Handles.RectangleHandleCap);
                        break;
                    case WayPointsMoveTool.CircleHandle:
                        //Creates a positionhandle in a crircle shape and set the target position to this handle.
                        targetPos = Handles.FreeMoveHandle(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, Quaternion.identity, 1, snap, Handles.CircleHandleCap);
                        break;
                }

                //Set the paypoint to the target position.
                s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value = targetPos;
            }
        }

        /// <summary> Connects every Waypoint with the Transform from the GameObject where the path is attached to. </summary>
        void ConnectMainTransformWithWaypoints()
        {
            //Set the Color of the Lines.
            Handles.color = Color.red;
            //Loops through all waypoints.
            for (int i = 0; i < s_WayPointsArray.arraySize; i++)
            {
                //Draws a line from the waypoint to the position of the GameObject.
                Handles.DrawAAPolyLine(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, path.transform.position);
                //Draws a disc around the waypoint.
                Handles.DrawWireDisc(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, new Vector3(0, 1, 0), 1);
            }
        }

        /// <summary> Draws the Index of each waypoint next to it. </summary>
        void DrawWayPointIndex()
        {
            //Loops through all waypoints and draw the label for the index of the waypoint.
            for (int i = 0; i < s_WayPointsArray.arraySize; i++)
                Handles.Label(s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value, i.ToString());
        }

        #endregion

        /// <summary> Lets the path move with the GameObject where the path is attached to. </summary>
        void SnapToParentObj()
        {
            //Check if the poition of the GameObject has changed
            if (lastPos != path.transform.position)
            {
                //Set the value of how much the position changed.
                Vector3 offset = lastPos - path.transform.position;

                //Subtracting the changed value from every waypoint and save that to the path. 
                for (int i = 0; i < s_WayPointsArray.arraySize; i++)
                {
                    s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value -= offset;
                    path.WayPoints[i] = s_WayPointsArray.GetArrayElementAtIndex(i).vector3Value;
                }
                //Set the last position of the GameObject to his current position to apply that the path doesn't change anymore when the GameObject won't be moved.
                lastPos = path.transform.position;
            }
        }

        #endregion

        #region Gizmos

        /// <summary> Draws the Gizmos inside of the scene view. </summary>
        /// <param name="target"> The path that is selected. </param>
        /// <param name="gizmoType"> When the gizmos should be drawn. </param>
        [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
        private static void OnDrawGizmos(Path target, GizmoType gizmoType)
        {
            //Check if there's a target and if it's not then return.
            if (target == null)
                return;

            //Create a bool wich stores if the flags of the path should be visible
            bool showFlag = target.showFlag;

            //Check if the flags should be visible and if there are waypoints to show the flags.
            if (showFlag && target.WayPoints.Length > 0)
            {
                //Draws a Flag Icon for every Waypoint.
                foreach (Vector3 vector in target.WayPoints)
                    Gizmos.DrawIcon(vector + new Vector3(0, .3f), FilePaths.GizmosRessourcesPath + "/FlagIcon.png");
            }
        }

        #endregion
    }
}
