using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class BattleScript
    {
        public static btBattle newBattle(IBattleConfigs configs/* in */, IBattleScripts scripts/* out */)
        {
            btBattle battle = new btBattle();

            scripts.moveScript = new btMoveScript();
            scripts.moveScript.Init(battle, configs, scripts);

            scripts.mainScript = new btMainScript();
            scripts.mainScript.Init(battle, configs, scripts);

            return battle;
        }
    }
}