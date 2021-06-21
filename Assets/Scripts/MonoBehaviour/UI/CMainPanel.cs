using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class CMainPanel : MonoBehaviour
{
    public CPlayerStatus PlayerStatus;
    public CPlayerMoney PlayerMoney;
    public CPlayerDiamond PlayerDiamond;
    public Button EnterBattleButton;

    void Start()
    {
        EnterBattleButton.onClick.AddListener(OnEnterBattleClick);

        PlayerStatus.Init();
        PlayerMoney.Init();
        PlayerDiamond.Init();
    }

    void OnEnterBattleClick()
    {
        var msg = new MsgEnterBattle();
        sc.pmServer.request(MsgType.PMEnterBattle, msg, true, r =>
        {
            if (r.err == ECode.Success)
            {
                var res = r.res as ResEnterBattle;
                //CBattleScene.resEnterBattle = res;
                //CBattleScene.enter();
                sc.bmServer.resEnterBattle = res;
                sc.bmServer.start();
            }
        },
        10000,
        retryOnReconnect: false);
    }
}
