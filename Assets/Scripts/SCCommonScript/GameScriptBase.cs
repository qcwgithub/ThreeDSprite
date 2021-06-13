using Data;

namespace Script
{
    public class GameScriptBase
    {
        public IGameConfigs configs;
        public IGameScripts scripts;

        public virtual void Init(IGameConfigs configs, IGameScripts scripts)
        {
            this.configs = configs;
            this.scripts = scripts;
        }
    }
}