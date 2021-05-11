using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCharacterMove : MonoBehaviour
{
    public CCharacter Char;
    public float Speed = 1f;

    private void Start()
    {
        this.Char.Pos = this.Char.transform.position;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0f || z != 0f)
        {
            this.Char.Skel.AnimationName = "run";
            this.Char.Skel.loop = true;

            CWalkable walkable = this.Char.CurrWalkable;
            Vector3 pos = walkable.Move(this.Char.Pos, this.Speed * Time.deltaTime * new Vector3(x, 0f, z));
            this.Char.Pos = pos;
        }
        else
        {
            this.Char.Skel.AnimationName = "idle";
            this.Char.Skel.loop = true;
        }
    }
}
