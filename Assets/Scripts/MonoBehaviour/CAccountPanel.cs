using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAccountPanel : MonoBehaviour
{
    public InputField inputField;
    public string account { get; private set; }

    void Awake()
    {
        this.inputField.text = LSUtils.GetString(LSKeys.PC_CHANNEL_USER_ID, "");
    }

    public void OnLoginClick()
    {
        string account = this.inputField.text;
        if (string.IsNullOrEmpty(account))
        {
            return;
        }

        this.account = account;
        LSUtils.SetString(LSKeys.PC_CHANNEL_USER_ID, account);
    }
}
