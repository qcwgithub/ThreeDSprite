using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSettingsPanel : MonoBehaviour
{
    public Text playerIdText;
    void Start()
    {
        this.playerIdText.text = sc.pmServer.playerId.ToString();
    }
    public void OnLogoutClick()
    {
        sc.mainScene.Logout();
    }
}
