using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class BtScene : MonoBehaviour
{
    public int Id;
    public BMBattleInfo battle { get; private set; }
    public bool DrawGizmos_ObjectBounds = false;
    public void Apply(BMBattleInfo battle)
    {
        this.battle = battle;
        BtObject[] cobjs = this.GetComponentsInChildren<BtObject>(true);
        for (int i = 0; i < cobjs.Length; i++)
        {
            BtObject cObj = cobjs[i];
            btObject lObj;
            battle.objects.TryGetValue(cObj.Id, out lObj);
            if (lObj == null)
            {
                Debug.LogError("lObj is null, id: " + cObj.Id);
                continue;
            }
            cObj.Apply(this, lObj);
        }
    }
}
