using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            case EType.Floor:
                {
                    var prop = serializedObject.FindProperty("FloorComposition");
                    FloorComposition comp = (FloorComposition)
                        EditorGUILayout.EnumPopup("Composition", (FloorComposition)prop.intValue);
                    prop.intValue = (int)comp;
                }
                break;
            case EType.Stair:
                {
                    var prop = serializedObject.FindProperty("StairDir");
                    StairDir dir = (StairDir)EditorGUILayout.EnumPopup("StairDir", (StairDir)prop.intValue);
                    prop.intValue = (int)dir;

                    prop = serializedObject.FindProperty("StairComposition");
                    StairComposition comp = (StairComposition)
                        EditorGUILayout.EnumPopup("Composition", (StairComposition)prop.intValue);
                    prop.intValue = (int)comp;
                }
                break;
            case EType.BoxObstacle:
                {
                    var prop = serializedObject.FindProperty("BoxObstacleComposition");
                    BoxObstacleComposition comp = (BoxObstacleComposition)
                        EditorGUILayout.EnumPopup("Composition", (BoxObstacleComposition)prop.intValue);
                    prop.intValue = (int)comp;
                }
                break;
        }

        if (script.Type == EType.Map)
        {
            if (GUILayout.Button("Save Map"))
                this.SaveMap(script);
            if (GUILayout.Button("Assign Children Id"))
                this.AssignChildrenId(script);
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void SaveMap(EObject script)
    {
        LMapData data = script.ToMapData();
        string text = JsonUtils.ToJson(data);
        File.WriteAllText("Assets/Resources/MapData/" + data.Id + ".txt", text, Encoding.UTF8);
        AssetDatabase.ImportAsset("Assets/Resources/MapData/" + data.Id + ".txt");

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
            Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);
            for (int j = 0; j < colliders.Length; j++)
            {
                DestroyImmediate(colliders[j]);
            }
        }
        
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

                default:
                    Debug.LogError("Unknown EType: " + obj.Type.ToString());
                    break;
            }
        }

        PrefabUtility.SaveAsPrefabAssetAndConnect(go, "Assets/Resources/MapPrefab/" + script.Id + ".prefab", InteractionMode.UserAction);
        DestroyImmediate(go);

        Debug.Log("Save map " + script.Id + " OK");
    }

    private void AssignChildrenId(EObject script)
    {
        HashSet<int> hs = new HashSet<int>();
        List<EObject> dups = new List<EObject>();
        int max = 0;
        EObject[] objs = script.GetComponentsInChildren<EObject>(true);
        foreach (var obj in objs)
        {
            if (obj.Id <= 0 || hs.Contains(obj.Id))
                dups.Add(obj);
            else
                hs.Add(obj.Id);
            if (obj.Id > max) max = obj.Id;
        }
        foreach (var dup in dups)
        {
            dup.Id = ++max;
        }
        Debug.Log("changed count: " + dups.Count);
    }
}
