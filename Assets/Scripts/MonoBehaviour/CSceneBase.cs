using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneBase : MonoBehaviour
{
    public Canvas Canvas;
    protected virtual bool CarePMConnection => true;

    protected virtual void Awake()
    {
        sc.currentScene = this;
        this.createLoadingPanel();
    }

    protected void createLoadingPanel()
    {
        GameObject go = GameObject.Instantiate<GameObject>(sc.LoadingPanelPrefab);
        sc.loadingPanel = go.GetComponent<CLoadingPanel>();
        go.GetComponent<RectTransform>().SetParent(this.Canvas.GetComponent<RectTransform>(), false);

        if (this.CarePMConnection)
        {
            sc.pmServer.OnStatusChange += this.onPMServerStatusChange;
            if (sc.pmServer.status != PMNetworkStatus.LoginToGSucceeded)
            {
                this.onPMServerStatusChange(sc.pmServer.status, sc.pmServer.statusMsg);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        sc.pmServer.OnStatusChange -= this.onPMServerStatusChange;
    }

    protected virtual void onPMServerStatusChange(PMNetworkStatus status, string message)
    {
        var info = sc.loadingPanel.show("login", -1);
        info.setMessage(status.ToString() + (message != null ? ", message: " + message : ""));

        switch (status)
        {
            case PMNetworkStatus.ConnectToAccount:
            case PMNetworkStatus.LoginToAccount:
            case PMNetworkStatus.LoginToASucceeded:
            case PMNetworkStatus.ConnectToGame:
            case PMNetworkStatus.LoginToG:
                info.setColor(Color.green);
                break;

            case PMNetworkStatus.LoginToGSucceeded:
                info.setColor(Color.green);
                sc.loadingPanel.hide("login");
                break;

            case PMNetworkStatus.ConnectToAFailed:
            case PMNetworkStatus.LoginToAFailed:
            case PMNetworkStatus.ConnectToGFailed:
            case PMNetworkStatus.LoginToGFailed:
            default:
                info.setColor(Color.red);
                break;
        }
    }
}
