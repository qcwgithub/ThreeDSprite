using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CStartupScene : CSceneBase
{
    public string NextSceneName;
    public CBoardPanel Board;
    public GameObject LoadingPanelPrefab;

    protected override void Awake()
    {
        sc.pmServer = new PMServer();
        sc.bmServer = new BMServer();
        sc.LoadingPanelPrefab = this.LoadingPanelPrefab;
        Board.gameObject.SetActive(false);
        base.Awake();
    }

    ServerList serverList = null;
    IEnumerator Start()
    {
        SDKManager.Instance.init();

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
            CMainScene.enter();
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
        var info = sc.loadingPanel.show("loadServerList", -1);
        info.setMessage("loading server list");
        string url = $"https://hecxxzdl.jysyx.net/server_list/{PlatformUtils.getPlatformString()}_{ProjectUtils.ProjectName}/{PlatformUtils.getAppVersion()}.txt";
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