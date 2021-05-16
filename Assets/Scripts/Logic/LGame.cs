using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGame : MonoBehaviour
{
    public CCharacter Char;
    public List<LMap> Maps;
    private void Start()
    {
        Application.targetFrameRate = 60;

        for (int i = 0; i < this.Maps.Count; i++)
        {
            this.Maps[i].Apply();
        }

        this.Maps[0].AddCharacter(this.Char);
    }

}
