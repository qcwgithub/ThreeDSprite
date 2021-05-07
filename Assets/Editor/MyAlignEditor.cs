using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AlignX
{
    None,
    EnterValue,
    BoxColliderSize_X,
}

public enum AlignY
{
    None,
    EnterValue,
    BoxColiderSize_Y,
}

public enum AlignZ
{
    None,
    EnterValue,
    BoxColliderSize_Z,
}

public class MyAlignEditor : EditorWindow
{
    [MenuItem("Window/My Align Window")]
    static void OpenMyAlignEditorWindow()
    {
        MyAlignEditor window = EditorWindow.GetWindow<MyAlignEditor>();
        window.Show();
    }

    public GameObject[] refs = new GameObject[0];
    public GameObject[] targets = new GameObject[0];

    public AlignX alignX = AlignX.None;
    public float xValue = 0f;
    public float multipleX = 1f;

    public AlignY alignY = AlignY.None;
    public float yValue = 0f;
    public float multipleY = 1f;

    public AlignZ alignZ = AlignZ.None;
    public float zValue = 0f;
    public float multipleZ = 1f;

    private bool GetRefVector(BoxCollider boxCollider, out Vector3 pos)
    {
        pos = Vector3.zero;

        float x = 0f;
        switch (this.alignX)
        {
            case AlignX.EnterValue:
                x = this.xValue;
                break;
            case AlignX.BoxColliderSize_X:
                if (boxCollider == null)
                {
                    Debug.Log("has no BoxCollider");
                    return false;
                }
                x = boxCollider.size.x;
                break;
        }
        x *= this.multipleX;

        float y = 0f;
        switch (this.alignY)
        {
            case AlignY.EnterValue:
                y = this.yValue;
                break;
            case AlignY.BoxColiderSize_Y:
                if (boxCollider == null)
                {
                    Debug.Log("has no BoxCollider");
                    return false;
                }
                y = boxCollider.size.y;
                break;
        }
        y *= this.multipleY;

        float z = 0f;
        switch (this.alignZ)
        {
            case AlignZ.EnterValue:
                z = this.zValue;
                break;
            case AlignZ.BoxColliderSize_Z:
                if (boxCollider == null)
                {
                    Debug.Log("has no BoxCollider");
                    return false;
                }
                z = boxCollider.size.z;
                break;
        }
        z *= this.multipleZ;

        pos = new Vector3(x, y, z);
        return true;
    }

    private void Align()
    {
        if (this.refs.Length <= 0)
        {
            Debug.Log("refs.Length == 0");
            return;
        }
        GameObject[] gos = this.targets;// Selection.gameObjects;
        if (gos.Length == 0)
        {
            Debug.Log("targets.Length == 0");
            return;
        }

        BoxCollider boxCollider = this.refs[0].GetComponentInChildren<BoxCollider>();
        Vector3 refVector;
        if (!this.GetRefVector(boxCollider, out refVector))
        {
            return;
        }
        

        Vector3 pos0 = this.refs[0].transform.position;
        for (int i = 1; i < gos.Length; i++)
        {
            gos[i].transform.position = pos0 + i * refVector;
        }
    }

    private void Clone()
    {
        if (this.refs.Length <= 0)
        {
            Debug.Log("refs.Length == 0");
            return;
        }
        //GameObject[] gos = this.targets;// Selection.gameObjects;
        //if (gos.Length == 0)
        //{
        //    Debug.Log("targets.Length == 0");
        //    return;
        //}

        for (int i = 0; i < this.refs.Length; i++)
        {
            GameObject goRef = this.refs[i];
            Transform transRef = goRef.transform;

            GameObject go = GameObject.Instantiate<GameObject>(goRef);
            Transform trans = go.transform;
            trans.SetParent(transRef.parent);

            BoxCollider boxCollider = goRef.GetComponentInChildren<BoxCollider>();
            Vector3 refVector;
            this.GetRefVector(boxCollider, out refVector);

            Vector3 pos0 = transRef.position;
            trans.position = pos0 + refVector;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        ScriptableObject scriptableObj = this;
        SerializedObject serialObj = new SerializedObject(scriptableObj);
        SerializedProperty serialProp = serialObj.FindProperty("refs");

        EditorGUILayout.PropertyField(serialProp);
        serialObj.ApplyModifiedProperties();
        GUILayout.EndHorizontal();

        if (this.refs.Length > 0 && GUILayout.Button("Clear Refs"))
        {
            this.refs = new GameObject[0];
        }

        GUILayout.BeginHorizontal();
        scriptableObj = this;
        serialObj = new SerializedObject(scriptableObj);
        serialProp = serialObj.FindProperty("targets");

        EditorGUILayout.PropertyField(serialProp);
        serialObj.ApplyModifiedProperties();
        GUILayout.EndHorizontal();
        if (this.targets.Length > 0 && GUILayout.Button("Clear Targets"))
        {
            this.targets = new GameObject[0];
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("multipleX", GUILayout.Width(60));
        this.multipleX = EditorGUILayout.FloatField(this.multipleX);
        EditorGUILayout.LabelField("AlignX", GUILayout.Width(60));
        this.alignX = (AlignX)EditorGUILayout.EnumPopup(this.alignX);
        if (this.alignX == AlignX.EnterValue)
        {
            this.xValue = EditorGUILayout.FloatField("", this.xValue);
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("multipleY", GUILayout.Width(60));
        this.multipleY = EditorGUILayout.FloatField(this.multipleY);
        EditorGUILayout.LabelField("AlignY", GUILayout.Width(60));
        this.alignY = (AlignY)EditorGUILayout.EnumPopup(this.alignY);
        if (this.alignY == AlignY.EnterValue)
        {
            this.yValue = EditorGUILayout.FloatField("", this.yValue);
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("multipleZ", GUILayout.Width(60));
        this.multipleZ = EditorGUILayout.FloatField(this.multipleZ);
        EditorGUILayout.LabelField("AlignZ", GUILayout.Width(60));
        this.alignZ = (AlignZ)EditorGUILayout.EnumPopup(this.alignZ);
        if (this.alignZ == AlignZ.EnterValue)
        {
            this.zValue = EditorGUILayout.FloatField("", this.zValue);
        }
        GUILayout.EndHorizontal();

        bool click = GUILayout.Button("Align");
        if (click)
        {
            this.Align();
        }

        click = GUILayout.Button("Clone");
        if (click)
        {
            this.Clone();
        }
    }
}
