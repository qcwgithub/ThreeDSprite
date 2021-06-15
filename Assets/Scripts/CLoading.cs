using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CLoading : MonoBehaviour
{
    public CBoard Board;
    public CLogin Login;

    void Awake()
    {
        Board.gameObject.SetActive(false);
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
            yield return Login.StartLogin(serverList.aaaIp, serverList.aaaPort);
        }

        if (serverList.enterGame)
        {
            SceneManager.LoadScene("Main");
            yield break;
        }
    }

    IEnumerator downloadServerList()
    {
        string url = $"https://hecxxzdl.jysyx.net/server_list/{PlatformUtils.getPlatformString()}_{ProjectUtils.ProjectName}/{PlatformUtils.getAppVersion()}.txt";
        Debug.Log(url);

        while (true)
        {
            var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                try
                {
                    string text = www.downloadHandler.text;
                    Debug.Log(text);
                    this.serverList = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerList>(text);
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
