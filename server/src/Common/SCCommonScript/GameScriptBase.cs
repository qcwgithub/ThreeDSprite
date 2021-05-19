public class GameScriptBase {
    public IGameConfigs configs;
    public IGameScripts scripts;

    public GameScriptBase init(IGameConfigs c, IGameScripts s) {
        this.configs = c;
        this.scripts = s;
        return this;
    }
}