using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Data;

public class MessagePackInitializer
{
    static bool serializerRegistered = false;
    public static void Initialize()
    {
        if (!serializerRegistered)
        {
            StaticCompositeResolver.Instance.Register(
                 MessagePack.Resolvers.GeneratedResolver.Instance,
                 MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }
    }
}

public class CStartupScene : CSceneBase
{
    public TextAsset serverListTxt;
    public string NextSceneName;
    public CBoardPanel Board;
    public GameObject loadingPanelPrefab;
    public CAccountPanel accountPanel;

    public static void Enter()
    {
        CLoadingScene.EnterScene("Startup");
    }

    protected override void Awake()
    {
        MessagePackInitializer.Initialize();

        sc.pmServer = new PMServer();
        sc.bmServer = new BMServer();
        sc.LoadingPanelPrefab = this.loadingPanelPrefab;
        Board.gameObject.SetActive(false);
        base.Awake();
    }

    ServerList serverList = null;
    IEnumerator Start()
    {
        // Data.BMMsgMove bb = new Data.BMMsgMove();
        // bb.moveDir.x = 1.2f;
        // var bbbb = MessagePackSerializer.Serialize(bb);
        // var cc = MessagePackSerializer.Deserialize<Data.BMMsgMove>(bbbb);

        SDKManager.Instance.init();
        
        if (PlatformUtils.getPlatformString() == MyChannels.pc)
        {
            this.accountPanel.gameObject.SetActive(true);

            while (string.IsNullOrEmpty(this.accountPanel.account))
            {
                yield return new WaitForSeconds(0.01f);
            }

            (SDKManager.Instance.getLoginInterface(MyChannels.pc) as PcLogin).channelUserId = this.accountPanel.account;
        }
        else
        {
            this.accountPanel.gameObject.SetActive(false);
        }


        yield return this.downloadServerList();

        if (serverList.showBoard && serverList.board != null)
        {
            Board.Show(serverList.board);
        }

        while (true)
        {
            if (!SDKManager.Instance.sdkMgrInited)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                break;
            }
        }

        if (serverList.needLogin)
        {
            yield return this.login(serverList.aaaIp, serverList.aaaPort);
        }

        if (serverList.enterGame)
        {
            CMainScene.Enter();
        }
    }

    IEnumerator login(string ip, int port)
    {
        sc.pmServer.aaaIp = ip;
        sc.pmServer.aaaPort = port;
        sc.pmServer.start();
        while (sc.pmServer.status != PMNetworkStatus.LoginToGSucceeded)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator downloadServerList()
    {
        if (PlatformUtils.getPlatformString() == MyChannels.pc)
        {
            this.serverList = Script.JsonUtils.parse<ServerList>(this.serverListTxt.text);
            yield break;
        }

        var info = sc.loadingPanel.show("loadServerList", -1);
        info.setMessage("loading server list");
        string url = $"https://hecxxzdl.jysyx.net/server_list/{PlatformUtils.getPlatformString()}_{ProjectUtils.ProjectName}/{PlatformUtils.getAppVersion()}.txt?t=" + DateTime.Now.Ticks.ToString();
        Debug.Log(url);

        while (true)
        {
            var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.error != null)
            {
                Debug.LogError(www.error);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                try
                {
                    string text = www.downloadHandler.text;
                    Debug.Log(text);
                    this.serverList = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerList>(text);
                    sc.loadingPanel.hide("loadServerList");
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }
    }
}
