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

        this.battle = BattleScript.newBattle(this, this);

        this.mainScript.initBattle(mapData, tilesetConfigs);
        btCharacter character = this.mainScript.addCharacter();

        btIWalkable chWalkable;
        Vector3 chPos;
        this.moveScript.randomWalkable(out chWalkable, out chPos);
        character.walkable = chWalkable;
        character.pos = chPos;

        Debug.Log("Object count: " + battle.objects.Count);

        GameObject prefab = Resources.Load<GameObject>(this.mapPath);
        if (prefab == null)
        {
            Debug.LogError("load map error 2");
            return;
        }

        GameObject go = GameObject.Instantiate<GameObject>(prefab);
        BtScene cMap = go.GetComponent<BtScene>();
        cMap.Apply(battle);

        this.Character.Apply(character, cMap);
        this.InputManager.OnInput += (Vector3 dir) =>
        {
            if (character.walkable == null)
            {
                dir.y = -1f;
            }
            if (dir != Vector3.zero)
            {
                // Vector3 delta = this.Speed * Time.deltaTime * dir;
                this.moveScript.characterMove(character, dir);
            }
            else
            {
                this.moveScript.characterStopMove(character);
            }
        };
    }

    private void Update()
    {
        this.mainScript.update(Time.deltaTime);
    }

    void OnDestroy()
    {
        this.mainScript.destroyBattle(this.battle);
    }
}
