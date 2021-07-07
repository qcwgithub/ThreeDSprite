using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerStatus : MonoBehaviour
{
    public Image portrait;
    public Text nameText;
    public Text levelText;
    public Slider expSlider;

    public void Init()
    {
        Refresh();
    }

    public void Refresh()
    {
        // var profile = sc.Profile;

        // this.nameText.text = profile.userName;
        // levelText.text = "Lv." + profile.level.ToString();
    }
}
