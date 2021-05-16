using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CInputManager : MonoBehaviour
{
    public CCharacter Character;
    public float Speed = 5f;
    public LMap Map;

    void Update()
    {
        Vector3 dir = Vector3.zero;
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        if (Character.Walkable == null)
        {
            dir.y = -1f;
        }
        Vector3 delta = this.Speed * Time.deltaTime * dir;
        this.Map.Move(this.Character, delta);
    }
}
