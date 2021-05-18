using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMap : MonoBehaviour
{
    public int Id;
    public LMap lMap { get; private set; }
    public void Apply(LMap lMap)
    {
        this.lMap = lMap;
        CObject[] cobjs = this.GetComponentsInChildren<CObject>(true);
        for (int i = 0; i < cobjs.Length; i++)
        {
            CObject cObj = cobjs[i];
            LObject lObj = lMap.GetObject(cObj.Id);
            if (lObj == null)
            {
                Debug.LogError("lObj is null, id: " + cObj.Id);
                continue;
            }
            cObj.Apply(lObj);
        }
    }
}
