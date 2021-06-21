using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CLoadingScene : CSceneBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        this.StartCoroutine(this.doEnterScene(s_sceneName));
    }
    
    IEnumerator doEnterScene(string sceneName)
    {
        Debug.Log("CLoadingScene.doEnterScene " + sceneName);
        sc.loadingPanel.show("loadScene", -1).setMessage("loading scene: " + sceneName);
        var loadOp = SceneManager.LoadSceneAsync(sceneName);
        yield return loadOp;
        // sc.loadingPanel.hide("loadScene");
    }

    static string s_sceneName;
    public static void s_enterScene(string sceneName)
    {
        Debug.Log("CLoadingScene.s_enterScene " + sceneName);
        s_sceneName = sceneName;
        SceneManager.LoadScene("Loading");
    }
}
