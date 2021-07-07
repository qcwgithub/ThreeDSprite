using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerMoney : MonoBehaviour
{
    public Text Money;
    public void Init()
    {
        Refresh();
    }
    public void Refresh()
    {
        // var profile = sc.Profile;
        // Money.text = profile.money.ToString();
    }
}
