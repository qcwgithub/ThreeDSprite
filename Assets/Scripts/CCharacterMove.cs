using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCharacterMove : MonoBehaviour
{
    public CCharacter Char;
    public float Speed = 1f;
    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0f || z != 0f)
        {
            CWalkable walkable = this.Char.CurrWalkable;
            Vector3 pos = walkable.Move(this.Char.transform.position, this.Speed * Time.deltaTime * new Vector3(x, 0f, z));
            this.Char.transform.position = pos;
        }
    }
}
