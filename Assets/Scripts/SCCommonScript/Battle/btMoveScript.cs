using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btMoveScript : btScriptBase
    {
        public ECode characterMove(btCharacter character, Vector3 moveDir)
        {
            if (!(moveDir.x >= -1f && moveDir.x <= 1f && moveDir.y >= -1f && moveDir.y <= 1f && moveDir.z >= -1f && moveDir.z <= 1f))
            {
                return ECode.InvalidParam;
            }
            
            character.moveDir = moveDir;
            return ECode.Success;
        }

        public void update(btBattle battle, float dt)
        {
            
        }
    }
}