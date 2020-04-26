using UnityEngine;
using UnityEditor;
using technical.test.editor;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;
using System.Collections.Generic;

public class GizmoEditorWindow : EditorWindow
{
    //fields
    public GizmoAsset gizmoData; //Asset selected

    public static int selectedIndex = -1; //ID of the selected gizmo
    private static GUILayoutOption layout; //Layout of the inputfields
    public static GizmoEditorWindow Instance { get; set; }

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
        return OpenGizmoAsset(instanceID);
    }

    //Utility function to open the window with a specific asset values
    public static bool OpenGizmoAsset(int instanceID)
    {
        selectedIndex = -1;
        GizmoAsset tmpGizmo = EditorUtility.InstanceIDToObject(instanceID) as GizmoAsset;
        if (tmpGizmo != null)
        {
            GizmoEditorWindow window = OpenGizmoEditorWindow();
            window.minSize = new Vector2(600, 1);
            window.maxSize = new Vector2(600, 10000);
            window.gizmoData = tmpGizmo;
            SceneView.RepaintAll();
            return true;
        }
        return false;
    }

    //Sets gizmoAsset to null and clear the window content
    public void ClearContent()
    {
        gizmoData = null;
        selectedIndex = -1;
        Instance.Repaint();
    }
   

    //Window GUI update
    private void OnGUI()
    {
        if (gizmoData != null)
        {
            for (int i = 0; i < gizmoData._gizmos.Length; i++)
            {
                //Color the selected line in red
                if (i == selectedIndex)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                gizmoData._gizmos[i].Name = EditorGUILayout.TextField(gizmoData._gizmos[i].Name, GUILayout.MinWidth(100));
                EditorGUILayout.Space();
                gizmoData._gizmos[i].Position = EditorGUILayout.Vector3Field("", gizmoData._gizmos[i].Position, GUILayout.MinWidth(300));
                EditorGUILayout.Space();
                if (GUILayout.Button("Edit", GUILayout.MinWidth(100)))
                {
                    EditButtonHandler(i);
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

            }
        }
    }

    private void EditButtonHandler(int index)
    {
        if (index == selectedIndex)
            selectedIndex = -1;
        else
            selectedIndex = index;

        SceneView.RepaintAll();
    }
}