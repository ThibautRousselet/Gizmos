using UnityEngine;
using UnityEditor;

//Custom Editor window for the gizmo asset
//Contains input fields for every gizmos properties

public class GizmoEditorWindow : EditorWindow
{
    //fields
    private static GUILayoutOption layout; //Layout of the inputfields
    public static GizmoEditorWindow Instance { get; set; }

    private Vector2 scrollPosition = Vector2.zero;

    //functions
    public static bool IsOpen
    {
        get { return Instance != null; }
    }

    //Keep reference to the window and set fontsize
    private void OnEnable()
    {
        Instance = this;
    }


    //Access window with menu tabs
    [MenuItem("Window/Custom/Gizmo Editor")]
    public static GizmoEditorWindow OpenGizmoEditorWindow()
    {
        return GetWindow<GizmoEditorWindow>();
    }

    //Access window by double clicking asset
    [UnityEditor.Callbacks.OnOpenAsset()]
    public static bool OnClickGizmoAsset(int instanceID, int line)
    {
        return OpenGizmoAsset();
    }

    //Utility function to open the window
    public static bool OpenGizmoAsset()
    {
         GizmoEditorWindow window = OpenGizmoEditorWindow();
         window.minSize = new Vector2(600, 1);
         SceneView.RepaintAll();
         return true;
    }
   

    //Window GUI update
    private void OnGUI()
    {
        if (GizmoCustomEditor._gizmos != null)
        {
            float fieldWidth = position.width / 5.0f;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            //Columns names
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("name", GUILayout.MinWidth(100), GUILayout.MaxWidth(fieldWidth));
            EditorGUILayout.LabelField(" ", GUILayout.MinWidth(100), GUILayout.MaxWidth(fieldWidth));
            EditorGUILayout.LabelField("position", GUILayout.MinWidth(100), GUILayout.MaxWidth(fieldWidth));
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < GizmoCustomEditor._gizmos.arraySize; i++)
            {
                //Color the selected line in red
                if (i == GizmoCustomEditor.selectedIndex)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;

                
                //Input fields
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                GizmoCustomEditor.SetNameAt(i, EditorGUILayout.TextField(GizmoCustomEditor.GetNameAt(i), GUILayout.MinWidth(100), GUILayout.MaxWidth(fieldWidth)));
                EditorGUILayout.Space();
                GizmoCustomEditor.SetPositionAt(i, EditorGUILayout.Vector3Field("", GizmoCustomEditor.GetPositiontAt(i), GUILayout.MinWidth(300), GUILayout.MaxWidth(fieldWidth * 3)));
                EditorGUILayout.Space();
                if (GUILayout.Button("Edit", GUILayout.MinWidth(100), GUILayout.MaxWidth(fieldWidth)))
                {
                    EditButtonHandler(i);
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            GUILayout.EndScrollView();
        }
    }

    //Handler of edit button. Set the index of the selected gizmo and save the initial position
    private void EditButtonHandler(int index)
    {
        if (index == GizmoCustomEditor.selectedIndex)
            GizmoCustomEditor.selectedIndex = -1;
        else
            GizmoCustomEditor.selectedIndex = index;

        //Save the position when entering edit mode
        GizmoCustomEditor.StartingPosition = GizmoCustomEditor.GetPositiontAt(index);
        SceneView.RepaintAll();
    }
}