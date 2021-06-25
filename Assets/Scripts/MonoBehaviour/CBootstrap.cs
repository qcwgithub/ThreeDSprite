using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBootstrap : MonoBehaviour
{
    public string MapPath = "Assets/Resources/Imported/Egzd/map1";
    public int MapId;
    public CInputManager InputManager;
    public BtCharacter Character;
    public float Speed = 5f;

    private BtScene Map;
    void Start()
    {
        Application.targetFrameRate = 60;

        Debug.Log("Loading map " + this.MapId + "...");

        TextAsset textAsset = Resources.Load<TextAsset>("MapData/" + this.MapId);
        if (textAsset == null)
        {
            Debug.LogError("map data not found, mapid = " + this.MapId);
            return;
        }

        btSceneData mapData = JsonUtils.FromJson<btSceneData>(textAsset.text);
        btScene scene = new btScene(mapData);
        btCharacter lChar = new btCharacter(scene, 10000);
        scene.AddCharacter(lChar);

        Debug.Log("Object count: " + scene.DictObjects.Count);

        GameObject prefab = Resources.Load<GameObject>("MapPrefab/" + this.MapId);
        if (prefab == null)
        {
            Debug.LogError("map prefab not found, mapid = " + this.MapId);
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
        };
    }

    private void Update()
    {
        
    }
}
