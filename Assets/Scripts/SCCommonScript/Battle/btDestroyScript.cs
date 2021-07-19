using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btDestroyScript : btScriptBase
    {
        public void destroyBattle(BMBattle battle)
        {
            Qu3eApi.SceneDestroy(battle.physicsScene);
            battle.physicsScene = IntPtr.Zero;
        }
    }
}