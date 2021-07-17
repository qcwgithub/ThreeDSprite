using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBattleSettingsPanel : MonoBehaviour
{
    public Text battleIdText;
    public Text mapIdText;

    void Start()
    {
        this.battleIdText.text = BMServer.resEnterBattle.battleId.ToString();
        this.mapIdText.text = BMServer.resEnterBattle.mapId.ToString();
    }

    public void OnExitClick()
    {
        sc.battleScene.Logout();
    }
}
