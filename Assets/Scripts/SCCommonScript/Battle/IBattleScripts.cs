using System.Collections;
using System.Collections.Generic;

namespace Script
{
    public interface IBattleScripts
    {
        btMoveSystem moveSystem { get; set; }
        btBattleInitSystem mainScript { get; set; }
    }
}