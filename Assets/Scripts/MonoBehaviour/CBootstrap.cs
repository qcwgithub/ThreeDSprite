using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Script;

public class CBootstrap : MonoBehaviour, IBattleScripts, IBattleConfigs
{
    const string baseDir = "Imported/Egzd";
    public string mapPath = baseDir + "/map1";
    public CInputManager InputManager;
    public BtCharacter Character;
    public float Speed = 5f;

    private BtScene Map;

    public btMoveScript moveScript { get; set; }
    public btMainScript mainScript { get; set; }

    btBattle battle;

    void Start()
    {
        Application.targetFrameRate = 60;

        Debug.Log("Loading map " + this.mapPath + ".tmx");
        TextAsset textAsset = Resources.Load<TextAsset>(this.mapPath + ".tmx");
        if (textAsset == null)
        {
            Debug.LogError("load map error 1");
            return;
        }

        btTilemapData mapData = JsonUtils.FromJson<btTilemapData>(textAsset.text);
        var tilesetConfigs = new Dictionary<string, btTilesetConfig>();
        for (int i = 0; i < mapData.layerDatas.Count; i++)
        {
            btTileLayerData layerData = mapData.layerDatas[i];

            for (int j = 0; j < layerData.tileDatas.Count; j++)
            {
                btTileData thingData = layerData.tileDatas[j];
                // string key = Path.GetFileNameWithoutExtension(aThing.tileset);

                btTilesetConfig tilesetConfig;
                if (!tilesetConfigs.TryGetValue(thingData.tileset, out tilesetConfig))
                {
                    string tilesetPath = baseDir + "/" + thingData.tileset;// + ".json";
                    TextAsset tilesetAsset = Resources.Load<TextAsset>(tilesetPath);
                    if (tilesetAsset == null)
                    {
                        Debug.LogError(tilesetPath + " not imported");
                        break;
                    }
                    tilesetConfig = JsonUtils.FromJson<btTilesetConfig>(tilesetAsset.text);
                    tilesetConfigs.Add(thingData.tileset, tilesetConfig);
                }
            }
        }

        var battleScript = new BattleScript();
        battleScript.createBattleScripts(this, this);

        this.battle = battleScript.createBattle(this, mapData, tilesetConfigs);
        btCharacter lChar = this.mainScript.addCharacter(battle);

        Debug.Log("Object count: " + battle.DictObjects.Count);

        GameObject prefab = Resources.Load<GameObject>(this.mapPath);
        if (prefab == null)
        {
            Debug.LogError("load map error 2");
            return;
        }

        GameObject go = GameObject.Instantiate<GameObject>(prefab);
        BtScene cMap = go.GetComponent<BtScene>();
        cMap.Apply(battle);

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
                this.moveScript.characterMove(lChar, delta);
            }
        };
    }

    private void Update()
    {
        this.moveScript.update(this.battle, Time.deltaTime);
    }
}
