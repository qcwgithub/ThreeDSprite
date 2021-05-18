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
        Debug.Log("Loading map " + this.MapId + "...");

        TextAsset textAsset = Resources.Load<TextAsset>("MapData/" + this.MapId);
        if (textAsset == null)
        {
            Debug.LogError("map data not found, mapid = " + this.MapId);
            return;
        }

        LMapData mapData = JsonUtils.FromJson<LMapData>(textAsset.text);
        LMap lMap = new LMap(mapData);

        GameObject prefab = Resources.Load<GameObject>("MapPrefab/" + this.MapId);
        if (prefab == null)
        {
            Debug.LogError("map prefab not found, mapid = " + this.MapId);
            return;
        }

        GameObject go = GameObject.Instantiate<GameObject>(prefab);
        CMap cMap = go.GetComponent<CMap>();
        cMap.Apply(lMap);

        //this.InputManager.OnInput += (Vector3 dir) =>
        //{
        //    Vector3 delta = this.Speed * Time.deltaTime * dir;
        //    this.Map.Move(this.Character, delta);
        //};
    }
}
