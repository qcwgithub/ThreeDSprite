using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CWalkableJoint))]
public class CWalkableJointInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add BoxCollider"))
        {
            CWalkableJoint script = this.target as CWalkableJoint;

            GameObject go = script.gameObject;
            BoxCollider boxCollider = go.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = go.AddComponent<BoxCollider>();
            }
            boxCollider.isTrigger = true;

            switch (script.Sides)
            {
                case WalkableJointSides.Left_Right:
                    boxCollider.size = new Vector3(1f, 0.5f, 0.5f);
                    break;
                case WalkableJointSides.Front_Back:
                    boxCollider.size = new Vector3(0.5f, 0.5f, 1f);
                    break;
            }
        }
    }
}
