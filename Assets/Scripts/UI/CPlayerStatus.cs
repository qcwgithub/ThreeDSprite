using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerStatus : MonoBehaviour
{
    public Image Portrait;
    public Text Name;
    public Text Level;
    public Slider ExpSlider;

    public void Init()
    {
        Refresh();
    }

    public void Refresh()
    {
        var profile = sc.Profile;

        Name.text = profile.userName;
        Level.text = "Lv." + profile.level.ToString();
    }
}
