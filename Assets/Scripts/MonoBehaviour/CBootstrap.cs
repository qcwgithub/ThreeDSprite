using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Script;

public class CBootstrap : MonoBehaviour, IBattleScripts, IBattleConfigs
{
    const string baseDir = "Imported/Egzd";
    // public string mapPath = baseDir + "/map1";
    public int mapId = 1;
    public CInputManager InputManager;
    public BtCharacter Character;
    public float Speed = 5f;

    private BtScene Map;

    public Dictionary<int, btTilemapData> tilemapDatas { get; private set; }
    public Dictionary<string, btTilesetConfig> tilesetConfigs { get; private set; }

    public btMoveScript moveScript { get; set; }
    public btMainScript mainScript { get; set; }

    btBattle battle;

    void Start()
    {
        Application.targetFrameRate = 60;

        BattleScript.initBattleScripts(this, this);
        BattleScript.loadMap(new Script.JsonUtils(), this, this.mapId,
            mapId =>
            {
                string mapPath = baseDir + "/map" + mapId;
                Debug.Log("Loading map " + mapPath + ".tmx");
                TextAsset textAsset = Resources.Load<TextAsset>(mapPath + ".tmx");
                if (textAsset == null)
                {
                    throw new Exception("load map error 1");
                }
                return textAsset.text;
            },
            tileset =>
            {
                string tilesetPath = baseDir + "/" + tileset;// + ".json";
                TextAsset tilesetAsset = Resources.Load<TextAsset>(tilesetPath);
                if (tilesetAsset == null)
                {
                    throw new Exception(tilesetPath + " not imported");
                }
                return tilesetAsset.text;
            });

        this.battle = this.mainScript.newBattle(this.mapId);
        btCharacter character = this.mainScript.addCharacter(this.battle);

        btIWalkable chWalkable;
        Vector3 chPos;
        this.moveScript.randomWalkable(this.battle, out chWalkable, out chPos);
        character.walkable = chWalkable;
        character.pos = chPos;

        Debug.Log("Object count: " + battle.objects.Count);

        string mapPath = baseDir + "/map" + this.mapId;
        GameObject prefab = Resources.Load<GameObject>(mapPath);
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
        this.mainScript.update(this.battle, Time.deltaTime);
    }

    void OnDestroy()
    {
        this.mainScript.destroyBattle(this.battle);
    }
}
