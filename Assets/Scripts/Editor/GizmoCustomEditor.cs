using UnityEngine;
using UnityEditor;
using technical.test.editor;

[CustomEditor(typeof(GizmoAsset))]
public class GizmoCustomEditor : Editor
{
    private static GUIStyle style = new GUIStyle(); // Font of text displayed
    private static Vector3 height = new Vector3(0, 3, 0); //Height of lines drawn
    private static float radius = 2.5f; //Radius of gizmo sphere
    private GizmoAsset gizmoAsset;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawDefaultInspector();
        if (GUILayout.Button("Show Editor"))
        {
            GizmoEditorWindow.OpenGizmoAsset(target.GetInstanceID());
        }
    }

    protected void OnEnable()
    {
        gizmoAsset = (GizmoAsset)target;
        style.fontSize = 20;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    protected void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        GizmoEditorWindow.Instance.ClearContent();
    }

    //Return the ID of the gizmo pointed by the mouse
    private int GetMouseOverID()
    {
        //Draw a ray at the scene viiew position pointing at the cursor
        Vector3 mousePos = Event.current.mousePosition;
        mousePos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePos.y;
        Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);
        SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);

        //Move towards the direction of the ray
        for (int i = 0; i< 10000; i++) //The range of detection is limited by the number of iteration
        {
            ray.origin += i * 0.05f * ray.direction;
            for (int j = 0;  j < gizmoAsset._gizmos.Length; j++)
            {
                //At each step, check if the current position of hte origin is inside a gizmo (distance < raidus of sphere)
                if((gizmoAsset._gizmos[j].Position - ray.origin).magnitude < radius)
                {
                    //Return the ID of first gizmo reached
                    return j;
                }
            }
        }
        //No gizmo pointed
        return -1;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (GizmoEditorWindow.IsOpen)
        {
            //Draw the position handle of the selected gizmo
            if (GizmoEditorWindow.selectedIndex >= 0)
            {
                gizmoAsset._gizmos[GizmoEditorWindow.selectedIndex].Position = Handles.PositionHandle(gizmoAsset._gizmos[GizmoEditorWindow.selectedIndex].Position, Quaternion.identity);
            }

            //Draw the menu when right clickiing a gizmo
            if (Event.current.type == EventType.MouseDown)
            {
                int clickedID = GetMouseOverID();
                if (clickedID >=0 && Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Item 1"), false, Callback, clickedID);
                    menu.AddItem(new GUIContent("Item 2"), false, Callback, clickedID);
                    menu.ShowAsContext();
                }
            }
            

            GizmoEditorWindow.Instance.Repaint();
        }
    }
    static void Callback(object obj)
    {
        Debug.Log("Clicked : " + (int)obj);
    }

    //Draw every sphere gizmos and names
    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    public static void DrawInfo(GameObject gameObject, GizmoType gizmoType)
    {
        //Only draw when the window is opened
        if (GizmoEditorWindow.IsOpen && GizmoEditorWindow.Instance.gizmoData != null)
        {
            for (int i = 0; i < GizmoEditorWindow.Instance.gizmoData._gizmos.Length; i++)
            {
                //Sphere
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(GizmoEditorWindow.Instance.gizmoData._gizmos[i].Position, radius);
                //Text
                Gizmos.color = Color.black;
                Gizmos.DrawLine(GizmoEditorWindow.Instance.gizmoData._gizmos[i].Position, GizmoEditorWindow.Instance.gizmoData._gizmos[i].Position + height * 1.5f);
                Handles.Label(GizmoEditorWindow.Instance.gizmoData._gizmos[i].Position + height * 2f, GizmoEditorWindow.Instance.gizmoData._gizmos[i].Name, style);
            }
        }
    }
}
