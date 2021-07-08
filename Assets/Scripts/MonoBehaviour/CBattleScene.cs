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

    public Dictionary<int, btTilemapData> tilemapDatas { get; } = new Dictionary<int, btTilemapData>();
    public Dictionary<string, btTilesetConfig> tilesetConfigs { get; } = new Dictionary<string, btTilesetConfig> ();

    public btMoveScript moveScript { get; set; }
    public btMainScript mainScript { get; set; }

    btBattle battle;
    private BtScene Map;

    public int battleId;
    public int mapId;
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

    protected void Start()
    {
        Application.targetFrameRate = 60;

        BMResPlayerLogin res = sc.bmServer.resBM;
        this.battleId = res.battleId;
        this.myCharacerId = res.characterId;
        this.mapId = res.mapId;

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

        this.battle = this.mainScript.newBattle(this.mapId);

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

        for (int i = 0; i < res.battleData.characters.Count; i++)
        {
            ////
            MCharacter mc = res.battleData.characters[i];
            btCharacter character = this.mainScript.addCharacter(this.battle, mc.id);
            character.walkable = this.battle.walkables[mc.walkableId];
            character.pos = FVector3.ToVector3(mc.pos);

            ////
            GameObject char_go = GameObject.Instantiate(this.characterPrefab);
            char_go.transform.SetParent(this.characterPrefab.transform.parent, false);
            BtCharacter char_mono = char_go.GetComponent<BtCharacter>();
            char_mono.Apply(character, cMap);
            char_go.SetActive(true);

            ////
            if (mc.id == this.myCharacerId)
            {
                this.myCharacter = character;
            }
        }

        /////////////////////////////////////////////////////////////////////////////
        
        this.InputManager.OnInput += (Vector3 dir) =>
        {
            // if (myCharacter.walkable == null)
            // {
            //     dir.y = -1f;
            // }
            
            

            if (dir != Vector3.zero)
            {
                // Vector3 delta = this.Speed * Time.deltaTime * dir;
                // this.moveScript.characterMove(myCharacter, dir);
            }
            else
            {
                // this.moveScript.characterStopMove(myCharacter);
            }
        };
    }

    void Update()
    {
        this.mainScript.update(this.battle, Time.deltaTime);
    }

    protected override void OnDestroy()
    {
        this.mainScript.destroyBattle(this.battle);
        base.OnDestroy();
    }
}
