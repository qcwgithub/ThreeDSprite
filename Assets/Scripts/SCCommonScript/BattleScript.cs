﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class BattleScript
    {
        public static void createBattleScripts(IBattleConfigs configs/* in */, IBattleScripts scripts/* out */)
        {
            scripts.moveScript = new btMoveScript();
            scripts.moveScript.Init(configs, scripts);

            scripts.mainScript = new btMainScript();
            scripts.mainScript.Init(configs, scripts);
        }
    }
}