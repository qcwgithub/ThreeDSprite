using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneBase : MonoBehaviour
{
    public Canvas Canvas;

    protected virtual void Awake()
    {
        this.createLoadingPanel();
    }

    protected void createLoadingPanel()
    {
        GameObject go = GameObject.Instantiate<GameObject>(sc.LoadingPanelPrefab);
        sc.loadingPanel = go.GetComponent<CLoadingPanel>();
        go.GetComponent<RectTransform>().SetParent(this.Canvas.GetComponent<RectTransform>(), false);
    }
}
