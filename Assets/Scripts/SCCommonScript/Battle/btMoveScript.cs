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
            character.moveDir = moveDir;
            return ECode.Success;
        }

        public void update(btBattle battle, float dt)
        {
            
        }
    }
}