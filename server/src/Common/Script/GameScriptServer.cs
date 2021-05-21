using System;

public class GameScriptServer : GameScript
{

    DateTime baseDate = new DateTime(1970, 1, 1);
    public override int getTime()
    {
        return (int)(DateTime.Now - baseDate).TotalMilliseconds;
    }

    public override int getTodayTime(int hour, int minute, int second, int ms)
    {
        return new Date().setHours(hour, minute, second, ms);
    }

    public override int setHours(int timeMs, int hour, int minute, int second, int ms)
    {
        return new Date(timeMs).setHours(hour, minute, second, ms);
    }

    public void setPortrait(IProfileInput input, string portrait)
    {
        input.profile.portrait = portrait;
    }
}