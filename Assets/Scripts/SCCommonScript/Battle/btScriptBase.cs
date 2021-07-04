using System.Collections;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class btScriptBase
    {
        public btBattle battle;
        public IBattleConfigs configs;
        public IBattleScripts scripts;

        public virtual void Init(btBattle battle, IBattleConfigs configs, IBattleScripts scripts)
        {
            this.battle = battle;
            this.configs = configs;
            this.scripts = scripts;
        }
    }
}