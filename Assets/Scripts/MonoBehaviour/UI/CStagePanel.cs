using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class CStagePanel : MonoBehaviour
{
    public RectTransform characterParent;
    public Text characterNameText;

    GameObject characterInstance;
    void Awake()
    {
        
    }

    void Start()
    {
        this.RefreshCharacter();
    }

    void RefreshCharacter()
    {
        int characterConfigId = sc.profile.characterConfigId;
        // load character prefab
        CharacterConfig config = sc.game.GetCharacterConfig(characterConfigId);
        GameObject prefab = Resources.Load<GameObject>(config.prefab);
        if (prefab == null)
        {
            Debug.LogError("character prefab not exist: " + config.prefab);
            return;
        }

        if (this.characterInstance != null)
        {
            Destroy(this.characterInstance);
        }

        this.characterInstance = Instantiate(prefab);
        this.characterInstance.transform.SetParent(this.characterParent, false);
        this.characterNameText.text = config.name;
    }

    void ChangeCharacterTo(int id)
    {
        if (id < sc.game.minCharacterConfigId)
        {
            id = sc.game.maxCharacterConfigId;
        }
        else if (id > sc.game.maxCharacterConfigId)
        {
            id = sc.game.minCharacterConfigId;
        }

        var msg = new MsgChangeCharacter();
        msg.characterConfigId = id;
        sc.pmServer.request(MsgType.ChangeCharacter, msg, true, (MyResponse r) =>
        {
            if (r.err == ECode.Success)
            {
                var res = r.res as ResChangeCharacter;
                sc.game.gameScript.ChangeCharacterExecute(sc.game, msg, res);
                this.RefreshCharacter();
            }
        });
    }

    public void OnPrevClick()
    {
        this.ChangeCharacterTo(sc.profile.characterConfigId - 1);
    }

    public void OnNextClick()
    {
        this.ChangeCharacterTo(sc.profile.characterConfigId + 1);
    }
}
