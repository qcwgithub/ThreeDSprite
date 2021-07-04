using System.Collections;
using System.Collections.Generic;

namespace Script
{
    public interface IBattleScripts
    {
        btMoveScript moveScript { get; set; }
        btMainScript mainScript { get; set; }
    }
}