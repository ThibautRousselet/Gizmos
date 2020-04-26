using UnityEngine;
using UnityEditor;
using technical.test.editor;
using System.Collections.Generic;

//Custom editor of the gizmo asset.
//Handles modification in scene and display visuals elements

[CustomEditor(typeof(GizmoAsset))]
public class GizmoCustomEditor : Editor
{
    //fields
    private static GUIStyle style = new GUIStyle(); // Font of text displayed
    private static Vector3 height = new Vector3(0, 3, 0); //Height of lines drawn
    private static float radius = 2.5f; //Radius of gizmo sphere

    public static int selectedIndex = -1; //Index of the selected gizmo
    public static SerializedProperty _gizmos = null; //Gizmos properties
    public static Vector3 StartingPosition; //Starting position when editing a gizmo

    //functions

    //Draw editor button
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Show Editor"))
        {
            GizmoEditorWindow.OpenGizmoAsset();
        }
    }

    //Get gizmo values and initialize font
    protected void OnEnable()
    {
        _gizmos = serializedObject.FindProperty("_gizmos");
        style.fontSize = 20;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    //clear window and gizmo values
    protected void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        _gizmos = null;
        if (GizmoEditorWindow.IsOpen)
            GizmoEditorWindow.Instance.Repaint();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (GizmoEditorWindow.IsOpen)
        {
            //Draw the position handle of the selected gizmo
            if (selectedIndex >= 0)
            {
                SetPositionAt(selectedIndex, Handles.PositionHandle(GetPositiontAt(selectedIndex), Quaternion.identity));
            }

            //Draw the menu when right clickiing a gizmo
            if (Event.current.type == EventType.MouseDown)
            {
                int clickedIndex = GetMouseOverIndex();
                if (clickedIndex >=0 && Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    
                    if (clickedIndex == selectedIndex)//Only selected gizmo can be reset
                        menu.AddItem(new GUIContent("Reset position"), false, ResetPosition, clickedIndex);
                    else//Only not selected gizmos can be selected
                        menu.AddItem(new GUIContent("Edit Gizmo"), false, SelectGizmo, clickedIndex);

                    menu.AddItem(new GUIContent("Delete Gizmo"), false, DeleteGizmo, clickedIndex);
                    menu.ShowAsContext();
                }
            }
            GizmoEditorWindow.Instance.Repaint();
        }
        serializedObject.ApplyModifiedProperties();
    }

    //Return the Index of the gizmo pointed by the mouse
    public static int GetMouseOverIndex()
    {
        //Draw a ray at the scene viiew position pointing at the cursor
        Vector3 mousePos = Event.current.mousePosition;
        mousePos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePos.y;
        Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);
        SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);


        float shortestDistance = float.MaxValue; //Distance to the closest detected gizmo
        int index = -1; //Index of the closest detected gizmo

        for (int j = 0; j < _gizmos.arraySize; j++)
        {
            //Check if the distance between the ray and the center of the gizmo sphere <= radius (is ray inside sphere)
            if (Vector3.Cross(ray.direction, GetPositiontAt(j) - ray.origin).magnitude <= radius)
            {
                //Select only the closest gizmo to the camera if many are detected
                if ((GetPositiontAt(j) - ray.origin).magnitude < shortestDistance)
                {
                    shortestDistance = (GetPositiontAt(j) - ray.origin).magnitude;
                    index = j;
                }
            }
        }
        return index;
    }

    //Called from gizmo rightclick, reset position to the one before editing
    private static void ResetPosition(object clickedIndex)
    {
        SetPositionAt((int)clickedIndex, StartingPosition);
    }

    //Called from gizmo rightclick, set selected gizmo to the clicked one
    private static void SelectGizmo(object clickedIndex)
    {
        selectedIndex = (int)clickedIndex;
    }

    //Called from gizmo rightclick, delete clicked gizmo
    private static void DeleteGizmo(object clickedIndex)
    {
        int deleteIndex = (int)clickedIndex;
        _gizmos.DeleteArrayElementAtIndex(deleteIndex);

        if (deleteIndex < selectedIndex)
            selectedIndex--;
        else if (deleteIndex == selectedIndex)
            selectedIndex = -1;
    }

    //Draw every sphere gizmos and names
    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    public static void DrawInfo(GameObject gameObject, GizmoType gizmoType)
    {
        //Only draw when the window is opened
        if (GizmoEditorWindow.IsOpen && _gizmos != null)
        {
            for (int i = 0; i < _gizmos.arraySize; i++)
            {
                //Sphere
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(GetPositiontAt(i), radius);
                //Text
                Gizmos.color = Color.black;
                Gizmos.DrawLine(GetPositiontAt(i), GetPositiontAt(i) + height * 1.5f);
                Handles.Label(GetPositiontAt(i) + height * 2f, GetNameAt(i), style);
            }
        }
    }

    //Getter and setter of serialized fields, avoid copy pasting
    public static Vector3 GetPositiontAt(int index)
    {
        if (_gizmos != null && index < _gizmos.arraySize)
            return _gizmos.GetArrayElementAtIndex(index).FindPropertyRelative("Position").vector3Value;
        return Vector3.zero;
    }
    public static void SetPositionAt(int index, Vector3 position)
    {
        if (_gizmos != null && index < _gizmos.arraySize)
            _gizmos.GetArrayElementAtIndex(index).FindPropertyRelative("Position").vector3Value = position;
    }

    public static string GetNameAt(int index)
    {
        if (_gizmos!=null && index < _gizmos.arraySize)
            return _gizmos.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue;
        return "";
    }
    public static void SetNameAt(int index, string name)
    {
        if (_gizmos != null && index < _gizmos.arraySize)
            _gizmos.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue = name;
    }
}
