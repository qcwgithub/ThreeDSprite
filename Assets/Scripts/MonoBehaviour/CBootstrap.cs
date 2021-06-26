using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBootstrap : MonoBehaviour
{
    public string mapPath = "Assets/Resources/Imported/Egzd/map1";
    public CInputManager InputManager;
    public BtCharacter Character;
    public float Speed = 5f;

    private BtScene Map;
    void Start()
    {/*
        Application.targetFrameRate = 60;

        Debug.Log("Loading map " + this.mapPath + "...");

        TextAsset textAsset = Resources.Load<TextAsset>(this.mapPath + "/.tmx");
        if (textAsset == null)
        {
            Debug.LogError("load map error 1");
            return;
        }

        btTilemapData mapData = JsonUtils.FromJson<btTilemapData>(textAsset.text);
        btScene scene = new btScene(mapData);
        btCharacter lChar = new btCharacter(scene, 10000);
        scene.AddCharacter(lChar);

        Debug.Log("Object count: " + scene.DictObjects.Count);

        GameObject prefab = Resources.Load<GameObject>(this.mapPath);
        if (prefab == null)
        {
            Debug.LogError("load map error 2");
            return;
        }

        GameObject go = GameObject.Instantiate<GameObject>(prefab);
        BtScene cMap = go.GetComponent<BtScene>();
        cMap.Apply(scene);

        lChar.Pos = this.Character.transform.position;
        this.Character.Apply(lChar, cMap);
        this.InputManager.OnInput += (Vector3 dir) =>
        {
            if (lChar.Walkable == null)
            {
                dir.y = -1f;
            }
            if (dir != Vector3.zero)
            {
                Vector3 delta = this.Speed * Time.deltaTime * dir;
                scene.Move(lChar, delta);
            }
        };*/
    }

    private void Update()
    {
        
    }
}
