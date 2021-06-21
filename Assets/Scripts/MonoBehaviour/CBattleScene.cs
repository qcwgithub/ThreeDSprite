using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.SceneManagement;

public class CBattleScene : CSceneBase
{
    protected override bool CarePMConnection => false;
    protected override void Awake()
    {
        sc.battleScene = this;
        base.Awake();
    }

    public static ResEnterBattle resEnterBattle;
    public static void enter()
    {
        CLoadingScene.s_enterScene("MapTest");
    }
}
