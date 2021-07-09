using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btUpdateScript : btScriptBase
    {
        public void update(BMBattleInfo battle, float dt)
        {
            this.scripts.moveScript.update(battle, dt);

            battle.updating = true;
            Qu3eApi.SceneStep(battle.physicsScene);
            battle.updating = false;
        }
    }
}