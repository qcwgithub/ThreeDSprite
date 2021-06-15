using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBoard : MonoBehaviour
{
    public Text title;
    public Text content;
    public Button button;

    void Awake()
    {
        this.button.onClick.AddListener(this.onButtonClick);
    }

    ServerList.BoardConfig boardConfig;
    public void Show(ServerList.BoardConfig boardConfig)
    {
        this.boardConfig = boardConfig;
        this.gameObject.SetActive(true);
        this.title.text = boardConfig.title;
        this.content.text = boardConfig.content;
        this.button.GetComponentInChildren<Text>().text = boardConfig.buttonText;
    }

    void onButtonClick()
    {
        Debug.Log("Clicked, action = " + this.boardConfig.buttonAction);
    }
}
