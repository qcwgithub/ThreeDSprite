using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainScene : CSceneBase
{
    public TextAsset characterCsv;

    protected override void Awake()
    {
        sc.mainScene = this;
        base.Awake();

        //...
        if (sc.game == null)
        {
            sc.game = new Game();
            sc.game.profile = sc.profile;            

            var dict = new Dictionary<string, TextAsset>
            {
                { "characters.csv", this.characterCsv },
            };

            sc.game.Load(file => dict[file].text);
        }
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
