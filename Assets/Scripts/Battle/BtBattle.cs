using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class BtBattle : MonoBehaviour
{
    public int Id;
    public BMBattle battle { get; private set; }
    public bool DrawGizmos_ObjectBounds = false;
    public void Apply(BMBattle battle)
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
