using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LObject), true)]
public class LObjectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set Id"))
        {
            LObject script = this.target as LObject;            
            LMap map = script.GetComponentInParent<LMap>();
            if (map == null)
            {
                Debug.Log("GetComponentInParent<CMap>() == null");
                return;
            }

            LObject[] walkables = map.GetComponentsInChildren<LObject>();
            script.Id = 1 + walkables.Max(w => w == script ? 0 : w.Id);
        }
    }
}
