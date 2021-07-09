using System.Collections;
using System.Collections.Generic;

namespace Script
{
    public interface IBattleScripts
    {
        btCreateScript createScript { get; set; }
        btMoveScript moveScript { get; set; }
        btMainScript mainScript { get; set; }
        btContactListenerScript contactListenerScript { get; set; }
        btDestroyScript destroyScript { get; set; }
        btUpdateScript updateScript { get; set; }
    }
}