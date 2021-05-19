using System;

public class GameScriptServer : GameScript {

    DateTime baseDate = new DateTime(1970, 1, 1);
    public int getTime() {
        return (int)(DateTime.Now - baseDate).TotalMilliseconds;
    }

    public int getTodayTime(int hour, int minute, int second, int ms) {

        return new Date().setHours(hour, minute, second, ms);
    }

    public int setHours(int timeMs, int hour, int minute, int second, int ms) {
        return new Date(timeMs).setHours(hour, minute, second, ms);
    }

    void setPortrait(IProfileInput input, string portrait) {
        input.profile.portrait = portrait;
    }
}