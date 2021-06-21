using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainScene : CSceneBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    public static void enter()
    {
        CLoadingScene.s_enterScene("Main");
    }
}
