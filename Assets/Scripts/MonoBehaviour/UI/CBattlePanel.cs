using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBattlePanel : MonoBehaviour
{
    public Text mapIdText;

    void Start()
    {
        this.mapIdText.text = BMServer.resEnterBattle.mapId.ToString();
    }
}
