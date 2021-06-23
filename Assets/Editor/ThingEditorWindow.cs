using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThingEditorWindow : EditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;

    [MenuItem("TDS/Thing Editor Window")]
    static void ShowWindow()
    {
        //GetWindowWithRect<ThingEditorWindow>(new Rect(0, 0, 256, 256));
        EditorWindow.GetWindow<ThingEditorWindow>();
    }

    void OnGUI()
    {
        gameObject = (GameObject) EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = null;

        if (gameObject != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject);

            //this.maxSize
            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
        }
    }
}
