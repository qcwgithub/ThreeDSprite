using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AlignXYZ
{
    NotChange,
    EnterValue,
    SrcBoxColliderSize_X,
    SrcBoxColliderSize_Y,
    SrcBoxColliderSize_Z,

    DstBoxColliderSize_X,
    DstBoxColliderSize_Y,
    DstBoxColliderSize_Z,
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

    public AlignXYZ alignX = AlignXYZ.NotChange; 
    public float xValue = 0f;
    public float multipleX = 1f;

    public AlignXYZ alignY = AlignXYZ.NotChange;
    public float yValue = 0f;
    public float multipleY = 1f;

    public AlignXYZ alignZ = AlignXYZ.NotChange;
    public float zValue = 0f;
    public float multipleZ = 1f;

    private float GetValue(AlignXYZ e, BoxCollider srcCollider, BoxCollider dstCollider)
    {
        switch (e)
        {
            case AlignXYZ.SrcBoxColliderSize_X:
            case AlignXYZ.SrcBoxColliderSize_Y:
            case AlignXYZ.SrcBoxColliderSize_Z:
                if (srcCollider == null)
                {
                    Debug.Log("srcCollider == null");
                    return 0f;
                }
                return srcCollider.size[e - AlignXYZ.SrcBoxColliderSize_X];
            //break;
            case AlignXYZ.DstBoxColliderSize_X:
            case AlignXYZ.DstBoxColliderSize_Y:
            case AlignXYZ.DstBoxColliderSize_Z:
                if (dstCollider == null)
                {
                    Debug.Log("dstCollider == null");
                    return 0f;
                }
                return dstCollider.size[e - AlignXYZ.DstBoxColliderSize_X];
                //break;
        }
        Debug.Log("unsupported e: " + e.ToString());
        return 0f;
    }

    private bool GetRefVector(BoxCollider srcCollider, BoxCollider dstCollider, out Vector3 pos, out bool[] flags)
    {
        flags = new bool[3] { true, true, true };
        pos = Vector3.zero;

        float x = 0f;
        switch (this.alignX)
        {
            case AlignXYZ.NotChange:
                flags[0] = false;
                break;
            case AlignXYZ.EnterValue:
                x = this.xValue;
                break;
            default:
                x = this.GetValue(this.alignX, srcCollider, dstCollider);
                break;
        }
        x *= this.multipleX;

        float y = 0f;
        switch (this.alignY)
        {
            case AlignXYZ.NotChange:
                flags[1] = false;
                break;
            case AlignXYZ.EnterValue:
                y = this.yValue;
                break;
            default:
                y = this.GetValue(this.alignY, srcCollider, dstCollider);
                break;
        }
        y *= this.multipleY;

        float z = 0f;
        switch (this.alignZ)
        {
            case AlignXYZ.NotChange:
                flags[2] = false;
                break;
            case AlignXYZ.EnterValue:
                z = this.zValue;
                break;
            default:
                z = this.GetValue(this.alignZ, srcCollider, dstCollider);
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

        if (this.targets.Length == 0)
        {
            Debug.Log("targets.Length == 0");
            return;
        }

        BoxCollider srcCollider = this.refs[0].GetComponentInChildren<BoxCollider>();
        

        Vector3 pos0 = this.refs[0].transform.position;
        for (int i = 0; i < this.targets.Length; i++)
        {
            BoxCollider dstCollider = this.targets[i].GetComponentInChildren<BoxCollider>();

            Vector3 refVector;
            bool[] flags;
            if (!this.GetRefVector(srcCollider, dstCollider, out refVector, out flags))
            {
                continue;
            }

            Vector3 pre = this.targets[i].transform.position;
            Vector3 curr = pos0 + (i+1) * refVector;
            if (!flags[0]) curr.x = pre.x;
            if (!flags[1]) curr.y = pre.y;
            if (!flags[2]) curr.z = pre.z;
            this.targets[i].transform.position = curr;
        }
    }

    //private void Clone()
    //{
    //    if (this.refs.Length <= 0)
    //    {
    //        Debug.Log("refs.Length == 0");
    //        return;
    //    }
    //    //GameObject[] gos = this.targets;// Selection.gameObjects;
    //    //if (gos.Length == 0)
    //    //{
    //    //    Debug.Log("targets.Length == 0");
    //    //    return;
    //    //}

    //    for (int i = 0; i < this.refs.Length; i++)
    //    {
    //        GameObject goRef = this.refs[i];
    //        Transform transRef = goRef.transform;

    //        GameObject go = GameObject.Instantiate<GameObject>(goRef);
    //        Transform trans = go.transform;
    //        trans.SetParent(transRef.parent);

    //        BoxCollider boxCollider = goRef.GetComponentInChildren<BoxCollider>();
    //        Vector3 refVector;
    //        this.GetRefVector(boxCollider, out refVector);

    //        Vector3 pos0 = transRef.position;
    //        trans.position = pos0 + refVector;
    //    }
    //}

    private void AverageBetween()
    {
        if (this.refs.Length < 2)
        {
            Debug.Log("refs.Length < 0");
            return;
        }
        if (this.targets.Length == 0)
        {
            Debug.Log("targets.Length == 0");
            return;
        }

        Vector3 p0 = this.refs[0].transform.position;
        Vector3 p1 = this.refs[1].transform.position;
        int L = this.targets.Length;
        for (int i = 0; i < L; i++)
        {
            this.targets[i].transform.position = Vector3.Lerp(p0, p1, (float)(i+1)/(float)(L + 1));
        }
    }

    public float XDistance = 1f;
    public int XCount = 10;
    public float ZDistance = 1f;
    private void MakeHorizontalGrid()
    {
        int L = this.targets.Length;
        if (L == 0)
        {
            Debug.Log("targets.Length == 0");
            return;
        }

        Vector3 p0 = this.targets[0].transform.position;
        Vector3 p = p0;
        int x_count = 1;
        for (int i = 0; i < L; i++)
        {

            this.targets[i].transform.position = p;
            x_count++;
            if (x_count >= this.XCount)
            {
                x_count = 0;
                p.x = p0.x;
                p.z -= this.ZDistance;
            }
            else
            {
                p.x += this.XDistance;
            }
        }
    }

    private Vector3 RandomTopLeft;
    private Vector3 RandomBottomRight;
    private void RandomTargetsInRect()
    {
        int L = this.targets.Length;
        if (L == 0)
        {
            Debug.Log("targets.Length == 0");
            return;
        }

        for (int i = 0; i < L; i++)
        {
            Vector3 p = new Vector3(Random.Range(this.RandomTopLeft.x, this.RandomBottomRight.x), this.RandomTopLeft.y,
                Random.Range(this.RandomTopLeft.z, this.RandomBottomRight.z));
            this.targets[i].transform.position = p;
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
        this.alignX = (AlignXYZ)EditorGUILayout.EnumPopup(this.alignX);
        if (this.alignX == AlignXYZ.EnterValue)
        {
            this.xValue = EditorGUILayout.FloatField("", this.xValue);
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("multipleY", GUILayout.Width(60));
        this.multipleY = EditorGUILayout.FloatField(this.multipleY);
        EditorGUILayout.LabelField("AlignY", GUILayout.Width(60));
        this.alignY = (AlignXYZ)EditorGUILayout.EnumPopup(this.alignY);
        if (this.alignY == AlignXYZ.EnterValue)
        {
            this.yValue = EditorGUILayout.FloatField("", this.yValue);
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("multipleZ", GUILayout.Width(60));
        this.multipleZ = EditorGUILayout.FloatField(this.multipleZ);
        EditorGUILayout.LabelField("AlignZ", GUILayout.Width(60));
        this.alignZ = (AlignXYZ)EditorGUILayout.EnumPopup(this.alignZ);
        if (this.alignZ == AlignXYZ.EnterValue)
        {
            this.zValue = EditorGUILayout.FloatField("", this.zValue);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Align"))
        {
            this.Align();
        }

        if (GUILayout.Button("Average Between Refs[0] and Refs[1]"))
        {
            this.AverageBetween();
        }

        this.XDistance = EditorGUILayout.FloatField("XDistance", this.XDistance);
        this.ZDistance = EditorGUILayout.FloatField("ZDistance", this.ZDistance);
        this.XCount = EditorGUILayout.IntField("XCount", this.XCount);
        if (GUILayout.Button("Make \'targets\' as horizontal grid"))
        {
            this.MakeHorizontalGrid();
        }

        this.RandomTopLeft = EditorGUILayout.Vector3Field("Random Top Left", this.RandomTopLeft);
        this.RandomBottomRight = EditorGUILayout.Vector3Field("Random Bottom Right", this.RandomBottomRight);
        if (GUILayout.Button("Random targets in rect"))
        {
            this.RandomTargetsInRect();
        }

        //click = GUILayout.Button("Clone");
        //if (click)
        //{
        //    this.Clone();
        //}
    }
}
