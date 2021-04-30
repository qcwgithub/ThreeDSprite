using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Assets.Editor
{
    [CustomEditor(typeof(MyImporter))]
    public class MyImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //base.OnInspectorGUI();



            var propPixelsPerUnit = serializedObject.FindProperty("pixelsPerUnit");
            propPixelsPerUnit.intValue = EditorGUILayout.IntField("Pixels Per Unit", propPixelsPerUnit.intValue);

            var propShape = serializedObject.FindProperty("shape");
            MyShape shape = (MyShape)EditorGUILayout.EnumPopup("Shape", (MyShape)propShape.intValue);
            propShape.intValue = (int)shape;

            
            switch (shape)
            {
                case MyShape.Cube:
                case MyShape.Top_Front:
                    {
                        var propBelowShape = serializedObject.FindProperty("belowPercent");
                        propBelowShape.floatValue = EditorGUILayout.Slider("Below Percent", propBelowShape.floatValue, 0f, 1f);
                    }
                    break;
                case MyShape.Quad:
                    {
                        var propQuadDir = serializedObject.FindProperty("quadDir");
                        propQuadDir.intValue = (int)(MyQuadDir)EditorGUILayout.EnumPopup("Quad Dir", (MyQuadDir)propQuadDir.intValue);
                    }
                    break;
            }

            var propPivot = serializedObject.FindProperty("pivot");
            propPivot.vector3Value = EditorGUILayout.Vector3Field("Pivot", propPivot.vector3Value);

            var propAddCollider = serializedObject.FindProperty("addCollider");
            propAddCollider.intValue = (int)(MyColliderType)EditorGUILayout.EnumPopup("Add Collider", (MyColliderType)propAddCollider.intValue);

            serializedObject.ApplyModifiedProperties();
            base.ApplyRevertGUI();

        }
    }
}
