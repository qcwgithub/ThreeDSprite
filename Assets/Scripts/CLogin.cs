using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CLogin : MonoBehaviour
{
    public Text LoginMessageLabel;

    public IEnumerator StartLogin(string ip, int port)
    {
        LoginMessageLabel.gameObject.SetActive(true);
        LoginMessageLabel.text = "";

        ClientServer.Instance = new RealServer();
        var realServer = (ClientServer.Instance as RealServer);
        realServer.OnStatusChange += this.OnStatusChange;
        realServer.start(ip, port);

        while (!loginSuccess)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    bool loginSuccess = false;
    void OnStatusChange(NetworkStatus status, string message)
    {
        LoginMessageLabel.text = status.ToString() + (message != null ? ", message: " + message : "");

        switch (status)
        {
            case NetworkStatus.ConnectToAccount:
            case NetworkStatus.LoginToAccount:
            case NetworkStatus.LoginToASucceeded:
            case NetworkStatus.ConnectToGame:
            case NetworkStatus.LoginToG:
                LoginMessageLabel.color = Color.green;
                break;

            case NetworkStatus.LoginToGSucceeded:
                var realServer = (ClientServer.Instance as RealServer);
                realServer.OnStatusChange -= this.OnStatusChange;
                LoginMessageLabel.color = Color.green;
                loginSuccess = true;
                break;

            case NetworkStatus.ConnectToAFailed:
            case NetworkStatus.LoginToAFailed:
            case NetworkStatus.ConnectToGFailed:
            case NetworkStatus.LoginToGFailed:
            default:
                LoginMessageLabel.color = Color.red;
                break;
        }
    }
}
