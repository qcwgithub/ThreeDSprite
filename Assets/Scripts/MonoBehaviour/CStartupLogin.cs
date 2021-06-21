using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CStartupLogin : MonoBehaviour
{
    public IEnumerator StartLogin(string ip, int port)
    {
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
        var info = sc.loadingPanel.show("login", -1);
        info.setMessage(status.ToString() + (message != null ? ", message: " + message : ""));

        switch (status)
        {
            case NetworkStatus.ConnectToAccount:
            case NetworkStatus.LoginToAccount:
            case NetworkStatus.LoginToASucceeded:
            case NetworkStatus.ConnectToGame:
            case NetworkStatus.LoginToG:
                info.setColor(Color.green);
                break;

            case NetworkStatus.LoginToGSucceeded:
                var realServer = (ClientServer.Instance as RealServer);
                realServer.OnStatusChange -= this.OnStatusChange;
                info.setColor(Color.green);
                loginSuccess = true;
                sc.loadingPanel.hide("login");
                break;

            case NetworkStatus.ConnectToAFailed:
            case NetworkStatus.LoginToAFailed:
            case NetworkStatus.ConnectToGFailed:
            case NetworkStatus.LoginToGFailed:
            default:
                info.setColor(Color.red);
                break;
        }
    }
}
