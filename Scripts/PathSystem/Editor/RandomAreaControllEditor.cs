using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Character.NPC.PathSystem
{
    /// <summary> Class to Edit a 'RandomAreaControll' in the inspector and ind the scene view. </summary>
    [CustomEditor(typeof(RandomAreaControll))]
    public class RandomAreaControllEditor : Editor
    {
        #region Variables/Enable

        /// <summary> Reference of the area wich is selected and should be shown/edited. </summary>
        RandomAreaControll areaControll;

        /// <summary> The BoxBoundsHandle to edit the bounds of the area. </summary>
        BoxBoundsHandle boundsHandle = new BoxBoundsHandle();
        /// <summary> Reference of the bounds of the area. </summary>
        Bounds boundsRef;
        /// <summary> True if the bounds can be edited. </summary>
        bool editHandle = false;
        /// <summary> The last position of the GameObject where the area is attached to. </summary>
        Vector3 lastPos;
        /// <summary> The color of the faces of the area. </summary>
        Color rectColorFace = new Color(.5f, .5f, .5f, .1f);
        /// <summary> The color of the edges of the area. </summary>
        Color rectColorOutline = new Color(.05f, .05f, .05f, .5f);

        //Array of Vector3 for every face that the area bounds has. 6 because it's a box.
        Vector3[] verts1;
        Vector3[] verts2;
        Vector3[] verts3;
        Vector3[] verts4;
        Vector3[] verts5;
        Vector3[] verts6;

        private void OnEnable()
        {
            //Initialize the areaControll and the bounds
            areaControll = (RandomAreaControll)target;
            boundsRef = areaControll.bounds;
            //Set the verts arrays to display the faces.
            GetVerts();
            //set the last position of the GameObject.
            lastPos = areaControll.transform.position;
        }

        #endregion

        #region InspectorGUI

        public override void OnInspectorGUI()
        {
            //Begin the Change Check.
            EditorGUI.BeginChangeCheck();

            //Draws a toggle button in the middle of a line to edit the bound of the area.
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("", GUILayout.MinWidth(50));
            editHandle = GUILayout.Toggle(editHandle, "Edit Area", "Button", GUILayout.MinWidth(100), GUILayout.MinHeight(25));
            EditorGUILayout.LabelField("", GUILayout.MinWidth(50));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            //Display the size and the center of the area bounds.
            boundsRef.size = EditorGUILayout.Vector3Field("Size", boundsRef.size);
            boundsRef.center = EditorGUILayout.Vector3Field("Center", boundsRef.center);

            //Check if there was made changes to the area inside of the inspector.
            if (EditorGUI.EndChangeCheck())
            {
                //Record the changes that was made.
                Undo.RecordObject(areaControll, "Change Area");

                //Save the bounds of the area and get the verts arrays to display the faces right
                areaControll.bounds = boundsRef;
                GetVerts();
            }
        }

        #endregion

        #region OnSceneGUI

        private void OnSceneGUI()
        {
            //loads the area bounds size and center.
            boundsHandle.center = areaControll.bounds.center;
            boundsHandle.size = areaControll.bounds.size;

            //Let the area move with the GameObject where it's attached to.
            SnapToParentObj();

            //Check if the area can be edited and if it does then call the 'EditBoundsHandle' method, otherwise just draw the area bounds by calling the 'DrawAreaBound' method.
            if (editHandle)
                EditBoundsHandle();
            else
                DrawAreaBound(Color.blue, rectColorFace, rectColorOutline);
        }

        /// <summary> Makes the area editable and save its changes. </summary>
        void EditBoundsHandle()
        {
            //Begin the ChangeCheck.
            EditorGUI.BeginChangeCheck();
            //Set the color of the area bounds.
            boundsHandle.SetColor(Color.magenta);
            //Let the area be editable.
            boundsHandle.DrawHandle();

            //Check if the area was changed.
            if (EditorGUI.EndChangeCheck())
            {
                //Record the changes made to the area.
                Undo.RecordObject(areaControll, "Change Area");

                //Save the bounds inside a new bounds.
                Bounds bounds = new Bounds(boundsHandle.center, boundsHandle.size);
                //Save the area boounds to from the new created bound.
                areaControll.bounds = bounds;
                //Set the bounds reference.
                boundsRef = areaControll.bounds;
                //Get the verts of the faces of the area to display them right.
                GetVerts();
            }
        }

        /// <summary> Draws rectangles for every face of the area and the egdes of it. </summary>
        /// <param name="Main"> The main color wich is used for the rectangles. </param>
        /// <param name="Face"> The color of the faces. </param>
        /// <param name="Outline"> The color of the edges. </param>
        void DrawAreaBound(Color Main, Color Face, Color Outline)
        {
            Handles.color = Main;
            Handles.DrawSolidRectangleWithOutline(verts1, Face, Outline);
            Handles.DrawSolidRectangleWithOutline(verts2, Face, Outline);
            Handles.DrawSolidRectangleWithOutline(verts3, Face, Outline);
            Handles.DrawSolidRectangleWithOutline(verts4, Face, Outline);
            Handles.DrawSolidRectangleWithOutline(verts5, Face, Outline);
            Handles.DrawSolidRectangleWithOutline(verts6, Face, Outline);
        }

        #endregion

        #region GetVerticalsFromArea

        /// <summary> Sets all faces of the area. </summary>
        void GetVerts()
        {
            //Sets all vertices off the area.
            Vector3 bv1 = boundsRef.min;
            Vector3 bv6 = boundsRef.max;
            Vector3 bv2 = new Vector3(bv1.x, bv1.y, bv6.z);
            Vector3 bv3 = new Vector3(bv1.x, bv6.y, bv6.z);
            Vector3 bv4 = new Vector3(bv1.x, bv6.y, bv1.z);
            Vector3 bv5 = new Vector3(bv6.x, bv6.y, bv1.z);
            Vector3 bv7 = new Vector3(bv6.x, bv1.y, bv6.z);
            Vector3 bv8 = new Vector3(bv6.x, bv1.y, bv1.z);

            //Sets the faces by the right verticis
            verts1 = new Vector3[4] { bv1, bv2, bv3, bv4 };
            verts2 = new Vector3[4] { bv1, bv8, bv5, bv4 };
            verts3 = new Vector3[4] { bv8, bv7, bv6, bv5 };
            verts4 = new Vector3[4] { bv7, bv6, bv3, bv2 };
            verts5 = new Vector3[4] { bv7, bv2, bv1, bv8 };
            verts6 = new Vector3[4] { bv6, bv3, bv4, bv5 };
        }

        #endregion

        /// <summary> Lets the area move with the GameObject where the area is attached to. </summary>
        void SnapToParentObj()
        {
            //Check if the position of the GameObject has changed
            if (lastPos != areaControll.transform.position)
            {
                //Store the value of the changes position range
                Vector3 offset = lastPos - areaControll.transform.position;

                //Subtracting the changed value from the center of the area bounds and save that to the area. 
                boundsRef.center -= offset;
                areaControll.bounds.center = boundsRef.center;
                //Get the verts of the faces of the area to display them right.
                GetVerts();

                //Set the last position of the GameObject to his current position to apply that the area doesn't change anymore when the GameObject won't be moved.
                lastPos = areaControll.transform.position;
            }
        }

        /// <summary> Draws gizmo inside of the scene view. </summary>
        /// <param name="target"> The target area that will be shown. </param>
        /// <param name="gizmoType"> When the gizmo will be shown. </param>
        [DrawGizmo(GizmoType.Active)]
        private static void OnDrawGizmos(RandomAreaControll target, GizmoType gizmoType)
        {
            //Check if there's a target and return if it isn't
            if (target == null)
                return;

            //Set the color of the Gizmos
            Gizmos.color = Color.red;
            //Get the bounds of the area.
            Bounds boundsRef = target.bounds;
            //Set the size of the cubes that will be drawn.
            Vector3 size = new Vector3(.5f, .5f, .5f);

            //Draw a cube at every vertice.
            Gizmos.DrawCube(boundsRef.min, size);
            Gizmos.DrawCube(boundsRef.max, size);
            Gizmos.DrawCube(new Vector3(boundsRef.min.x, boundsRef.min.y, boundsRef.max.z), size);
            Gizmos.DrawCube(new Vector3(boundsRef.min.x, boundsRef.max.y, boundsRef.max.z), size);
            Gizmos.DrawCube(new Vector3(boundsRef.min.x, boundsRef.max.y, boundsRef.min.z), size);
            Gizmos.DrawCube(new Vector3(boundsRef.max.x, boundsRef.max.y, boundsRef.min.z), size);
            Gizmos.DrawCube(new Vector3(boundsRef.max.x, boundsRef.min.y, boundsRef.max.z), size);
            Gizmos.DrawCube(new Vector3(boundsRef.max.x, boundsRef.min.y, boundsRef.min.z), size);
        }
    }
}
