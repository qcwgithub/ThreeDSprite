using System.Collections;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class btScriptBase
    {
        public IBattleConfigs configs;
        public IBattleScripts scripts;

        public virtual void Init(IBattleConfigs configs, IBattleScripts scripts)
        {
            this.configs = configs;
            this.scripts = scripts;
        }
    }
}