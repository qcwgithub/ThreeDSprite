using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.SceneManagement;
using Script;

public class CBattleScene : CSceneBase, IBattleScripts, IBattleConfigs
{
    protected override bool carePMConnection => false;
    
    const string baseDir = "Imported/Egzd";
    public float Speed = 5f;
    public CInputManager InputManager;
    public GameObject characterPrefab;

    ////////////////////////////////////////////////////////////////////////////////////////
    #region IBattleConfigs

    public Dictionary<int, btTilemapData> tilemapDatas { get; } = new Dictionary<int, btTilemapData>();
    public Dictionary<string, btTilesetConfig> tilesetConfigs { get; } = new Dictionary<string, btTilesetConfig> ();

    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////
    #region IBattleScripts

    public btMoveScript moveScript { get; set; }
    public btMainScript mainScript { get; set; }
    public btCreateScript createScript { get; set; }
    public btContactListenerScript contactListenerScript { get; set; }
    public btDestroyScript destroyScript { get; set; }
    public btUpdateScript updateScript { get; set; }

    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////

    public BMBattleInfo battle { get; private set; }

    private BtScene Map;

    public int battleId;
    public int mapId;
    public int myPlayerId;
    public int myCharacerId;
    public btCharacter myCharacter;

    protected override void Awake()
    {
        sc.battleScene = this;
        base.Awake();
    }

    public static void enter()
    {
        CLoadingScene.s_enterScene("Battle");
    }

    Vector3 lastInputDir = Vector3.zero;
    protected void Start()
    {
        Application.targetFrameRate = 60;

        this.myPlayerId = sc.pmServer.playerId;

        BMResPlayerLogin res = sc.bmServer.resBM;
        this.battleId = res.battle.battleId;
        this.mapId = res.battle.mapId;

        // myCharacter may be 0
        this.myCharacerId = 0;
        if (res.characterDict.ContainsKey(this.myPlayerId))
        {
            this.myCharacerId = res.characterDict[this.myPlayerId].characterId;
        }

        /////////////////////////////////////////////////////////////////////////////

        BattleScript.initBattleScripts(this, this);

        /////////////////////////////////////////////////////////////////////////////

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

        /////////////////////////////////////////////////////////////////////////////

        this.battle = this.createScript.newBattle(res.battle.battleId, res.battle.mapId);

        /////////////////////////////////////////////////////////////////////////////

        string prefabPath = baseDir + "/map" + this.mapId;
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("load map error 2");
            return;
        }

        GameObject go = GameObject.Instantiate<GameObject>(prefab);
        BtScene cMap = go.GetComponent<BtScene>();
        cMap.Apply(battle);

        /////////////////////////////////////////////////////////////////////////////

        foreach (var kv in res.characterDict)
        {
            int playerId = kv.Key;
            MCharacter mc = kv.Value;

            ////
            btCharacter character = this.mainScript.addCharacter(this.battle, mc.characterId, playerId, mc.pos);
            character.walkable = this.battle.walkables.Find(_ => ((btObject)_).id == mc.walkableId);

            ////
            GameObject char_go = GameObject.Instantiate(this.characterPrefab);
            char_go.transform.SetParent(this.characterPrefab.transform.parent, false);
            BtCharacter char_mono = char_go.GetComponent<BtCharacter>();
            char_mono.Apply(character, cMap);
            char_go.SetActive(true);

            ////
            if (mc.characterId == this.myCharacerId)
            {
                this.myCharacter = character;
            }
        }

        /////////////////////////////////////////////////////////////////////////////
        
        int id = 1;
        this.InputManager.OnInput += (Vector3 dir) =>
        {
            // if (myCharacter.walkable == null)
            // {
            //     dir.y = -1f;
            // }

            if (dir != this.lastInputDir)            
            {
                this.lastInputDir = dir;
                // Debug.LogFormat("send dir {0} {1},{2},{3}", id, dir.x, dir.y, dir.z);

                var msg = BMMsgMove.shared;
                msg.moveDir = dir;
                msg.id = id++;
                sc.bmServer.send(MsgType.BMMove, msg);
            }
        };
    }

    void Update()
    {
        this.updateScript.update(this.battle, Time.deltaTime);
    }

    protected override void OnDestroy()
    {
        this.destroyScript.destroyBattle(this.battle);
        base.OnDestroy();
    }
}
