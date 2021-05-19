public abstract class GameScript : GameScriptBase {
    public abstract int getTime();
    public abstract int getTodayTime(int hour, int minute, int second, int ms);
    public abstract int setHours(int timeMs, int hour, int minute, int second, int ms);
}