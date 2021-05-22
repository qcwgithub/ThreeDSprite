using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBootstrap : MonoBehaviour
{
    public int MapId;
    public CInputManager InputManager;
    public CCharacter Character;
    public float Speed = 5f;

    private CMap Map;
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

        LMapData mapData = JsonUtils.FromJson<LMapData>(textAsset.text);
        LMap lMap = new LMap(mapData);
        LCharacter lChar = new LCharacter(lMap, 10000);
        lMap.AddCharacter(lChar);

        Debug.Log("Object count: " + lMap.DictObjects.Count);

        GameObject prefab = Resources.Load<GameObject>("MapPrefab/" + this.MapId);
        if (prefab == null)
        {
            Debug.LogError("map prefab not found, mapid = " + this.MapId);
            return;
        }

        GameObject go = GameObject.Instantiate<GameObject>(prefab);
        CMap cMap = go.GetComponent<CMap>();
        cMap.Apply(lMap);

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
                lMap.Move(lChar, delta);
            }
        };
    }

    private void Update()
    {
        
    }
}
