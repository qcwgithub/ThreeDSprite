using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtScene : MonoBehaviour
{
    public int Id;
    public btScene scene { get; private set; }
    public bool DrawGizmos_ObjectBounds = false;
    public void Apply(btScene scene)
    {
        this.scene = scene;
        BtObject[] cobjs = this.GetComponentsInChildren<BtObject>(true);
        for (int i = 0; i < cobjs.Length; i++)
        {
            BtObject cObj = cobjs[i];
            btObject lObj = scene.GetObject(cObj.Id);
            if (lObj == null)
            {
                Debug.LogError("lObj is null, id: " + cObj.Id);
                continue;
            }
            cObj.Apply(this, lObj);
        }
    }
    
    private void Update()
    {
        this.scene.Update();
    }

    void OnDestroy()
    {
        this.scene.OnDestroy();
    }
}
