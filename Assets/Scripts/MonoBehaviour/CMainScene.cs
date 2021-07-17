using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainScene : CSceneBase
{
    protected override void Awake()
    {
        sc.mainScene = this;
        base.Awake();
    }

    public static void Enter()
    {
        CLoadingScene.EnterScene("Main");
    }

    public void Logout()
    {   
        Debug.Log("CMainScene.logout");
        SDKManager.Instance.onLogoutGame();

        sc.pmServer.OnDestroy();

        CStartupScene.Enter();
    }
}
