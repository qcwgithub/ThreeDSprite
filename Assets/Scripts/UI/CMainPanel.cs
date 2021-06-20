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
        ClientServer.Instance.request(MsgType.PMEnterBattle, msg, true, r =>
        {
            if (r.err == ECode.Success)
            {
                var res = r.res as ResEnterBattle;
            }
        },
        10000,
        retryOnReconnect: false);
    }
}
