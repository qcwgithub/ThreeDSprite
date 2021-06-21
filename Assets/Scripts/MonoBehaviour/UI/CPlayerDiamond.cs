using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerDiamond : MonoBehaviour
{
    public Text Diamond;
    public void Init()
    {
        Refresh();
    }
    public void Refresh()
    {
        var profile = sc.Profile;
        Diamond.text = profile.diamond.ToString();
    }
}
