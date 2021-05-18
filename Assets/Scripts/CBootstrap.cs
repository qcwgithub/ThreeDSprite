using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBootstrap : MonoBehaviour
{
    public int MapId;
    public CInputManager InputManager;
    public CCharacter Character;
    public float Speed = 5f;

    private CMap Map;
    void Start()
    {
        //this.InputManager.OnInput += (Vector3 dir) =>
        //{
        //    Vector3 delta = this.Speed * Time.deltaTime * dir;
        //    this.Map.Move(this.Character, delta);
        //};
    }
}
