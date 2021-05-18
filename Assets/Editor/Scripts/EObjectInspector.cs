using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EObject), true)]
public class EObjectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

            EObject script = this.target as EObject;

        serializedObject.Update();
        switch (script.Type)
        {
            case EType.Stair:
                {
                    var propShape = serializedObject.FindProperty("StairDir");
                    StairDir dir = (StairDir)EditorGUILayout.EnumPopup("StairDir", (StairDir)propShape.intValue);
                    propShape.intValue = (int)dir;
                }
                break;
        }
        switch (script.Type)
        {
            case EType.BoxObstacle:
                {
                    var prop = serializedObject.FindProperty("Walkable");
                    prop.boolValue = EditorGUILayout.Toggle("Walkable", prop.boolValue);
                }
                break;
        }
        switch (script.Type)
        {
            case EType.Floor:
            case EType.Stair:
                {
                    var prop = serializedObject.FindProperty("collider1");
                    EditorGUILayout.ObjectField(prop);
                    prop = serializedObject.FindProperty("collider2");
                    EditorGUILayout.ObjectField(prop);
                }
                break;
        }
        if (script.Type != EType.Map && GUILayout.Button("Assign Id"))
        {
            Transform parent = script.transform.parent;
            while (parent != null)
            {
                EObject obj = parent.GetComponent<EObject>();
                if (obj.Type == EType.Map)
                {
                    break;
                }
                parent = parent.parent;
            }
            if (parent == null)
            {
                Debug.Log("EObject with Type=Map not found");
                return;
            }

            EObject[] allObjects = parent.GetComponentsInChildren<EObject>();
            script.Id = 1 + allObjects.Max(w => {
                if (w == script)
                    return 0;
                if (w.Type == EType.Map)
                    return 0;

                return w.Id;
            }) ;
        }
        if (script.Type == EType.Map && GUILayout.Button("Save Map"))
        {
            this.SaveMap(script);
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void SaveMap(EObject script)
    {
        LMapData data = script.ToMapData();
        //Debug.Log(JsonUtils.ToJson(data));
        TextAsset asset = new TextAsset(JsonUtils.ToJson(data));
        AssetDatabase.DeleteAsset("Assets/Resources/MapData/" + data.Id + ".txt");
        AssetDatabase.CreateAsset(asset, "Assets/Resources/MapData/" + data.Id + ".txt");

        // prefab
        GameObject go = Instantiate<GameObject>(script.gameObject);
        go.name = script.name;

        go.transform.parent = script.gameObject.transform.parent;

        // 
        CMap map = go.AddComponent<CMap>();
        map.Id = go.GetComponent<EObject>().Id;
        DestroyImmediate(go.GetComponent<EObject>());

        EObject[] objs = go.GetComponentsInChildren<EObject>(false);
        for (int i = 0; i < objs.Length; i++)
        {
            EObject obj = objs[i];
            switch (obj.Type)
            {
                case EType.Floor:
                    {
                        CFloor floor = obj.gameObject.AddComponent<CFloor>();
                        floor.Id = obj.Id;
                        DestroyImmediate(obj);
                    }
                    break;

                case EType.Stair:
                    {
                        CStair stair = obj.gameObject.AddComponent<CStair>();
                        stair.Id = obj.Id;
                        DestroyImmediate(obj);
                    }
                    break;

                case EType.BoxObstacle:
                    {
                        CBoxObstacle ob = obj.gameObject.AddComponent<CBoxObstacle>();
                        ob.Id = obj.Id;
                        DestroyImmediate(obj);
                    }
                    break;
            }
        }

        PrefabUtility.SaveAsPrefabAssetAndConnect(go, "Assets/Resources/MapPrefab/" + script.Id + ".prefab", InteractionMode.UserAction);
        DestroyImmediate(go);
    }
}
