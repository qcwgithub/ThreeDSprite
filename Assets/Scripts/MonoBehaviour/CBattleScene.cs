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

    public const string baseDir = "Imported/Egzd";
    public CCameraFollow cameraFollow;
    public float Speed = 5f;
    public CInputManager InputManager;
    public Transform characterParent;

    ////////////////////////////////////////////////////////////////////////////////////////
    #region IBattleConfigs

    public Dictionary<int, btTilemapData> tilemapDatas { get; } = new Dictionary<int, btTilemapData>();
    public Dictionary<string, btTilesetConfig> tilesetConfigs { get; } = new Dictionary<string, btTilesetConfig>();

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

    public BMBattle battle;
    public BtBattle BtBattle { get; private set; }

    private BtBattle Map;

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

    public static void Enter()
    {
        CLoadingScene.EnterScene("Battle");
    }

    public void Logout()
    {
        sc.bmServer.OnDestroy();
        CMainScene.Enter();
    }

    Vector3 lastInputDir = Vector3.zero;
    protected void Start()
    {
        this.myPlayerId = sc.pmServer.playerId;

        // BMMsgBattle msg = sc.bmServer.resBM;
        // this.handleServerMessage(MsgType.

        sc.bmServer = new BMServer();
        sc.bmServer.OnStatusChange += this.onBMServerStatusChange;
        sc.bmServer.onServerMessage += this.handleServerMessage;
        sc.bmServer.start();

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


    IEnumerator loginBM()
    {
        sc.bmServer.start();
        while (sc.bmServer.status != BMNetworkStatus.LoginToBattleSucceeded)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    Dictionary<MsgType, OnMessageBase> handlerDict = new Dictionary<MsgType, OnMessageBase>();
    public void initHandlers()
    {
        this.handlerDict.Add(MsgType.BMBattle, new OnBMBattle());
        this.handlerDict.Add(MsgType.BMMove, new OnBMCharacterMove());
        this.handlerDict.Add(MsgType.BMAddPlayer, new OnBMAddPlayer());
        this.handlerDict.Add(MsgType.BMAddCharacter, new OnBMAddCharacter());
    }

    public void handleServerMessage(MsgType msgType, object msg)
    {
        if (this.handlerDict.Count == 0)
        {
            this.initHandlers();
        }
        OnMessageBase handler;
        if (this.handlerDict.TryGetValue(msgType, out handler))
        {
            handler.Handle(msg);
        }
        else
        {
            Debug.LogError("No handler for MsgType." + msgType);
        }
    }

    public void ApplyBattle(BMBattle battle)
    {
        /////////////////////////////////////////////////////////////////////////////

        string prefabPath = baseDir + "/map" + battle.mapId;
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("load map error 2");
            return;
        }

        GameObject go = GameObject.Instantiate<GameObject>(prefab);
        this.BtBattle = go.GetComponent<BtBattle>();
        this.BtBattle.Apply(battle);

        /////////////////////////////////////////////////////////////////////////////

        foreach (var kv in this.battle.characters)
        {
            btCharacter c = kv.Value;
            this.ApplyCharacter(c);
        }
    }

    public void ApplyCharacter(btCharacter character)
    {
        CharacterConfig characterConfig = sc.game.GetCharacterConfig(character.characterConfigId);
        
        GameObject prefab = Resources.Load<GameObject>(characterConfig.prefab_battle);
        if (prefab == null)
        {
            // Debug.LogError("character prefab not exist: " + characterConfig.prefab_battle);
            return;
        }

        ////
        GameObject char_go = GameObject.Instantiate(prefab);
        char_go.transform.SetParent(this.characterParent, false);
        BtCharacter char_mono = char_go.GetComponent<BtCharacter>();
        char_mono.Apply(character, this.BtBattle);
        char_go.SetActive(true);

        ////
        if (character.id == this.myCharacerId)
        {
            this.cameraFollow.Target = char_go.transform;
        }
    }

    void Update()
    {
        if (this.battle != null)
        {
            this.updateScript.update(this.battle, Time.deltaTime);
        }
    }

    protected override void OnDestroy()
    {
        sc.bmServer.OnStatusChange -= this.onBMServerStatusChange;
        this.destroyScript.destroyBattle(this.battle);
        base.OnDestroy();
    }

    protected void onBMServerStatusChange(BMNetworkStatus status, string message)
    {
        var info = sc.loadingPanel.show("login", -1);
        info.setMessage(status.ToString() + (message != null ? ", message: " + message : ""));

        switch (status)
        {
            case BMNetworkStatus.ConnectToBattle:
            case BMNetworkStatus.LoginToBattle:
                info.setColor(Color.green);
                break;

            case BMNetworkStatus.LoginToBattleSucceeded:
                info.setColor(Color.green);
                sc.loadingPanel.hide("login");
                break;

            case BMNetworkStatus.ConnectToBattleFailed:
            case BMNetworkStatus.LoginToBattleFailed:
            default:
                info.setColor(Color.red);
                break;
        }
    }
}
