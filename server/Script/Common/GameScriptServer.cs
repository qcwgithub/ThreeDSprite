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
        DateTime time = DateTime.Today.AddMilliseconds(hour * 3600000 + minute * 60000 + second * 1000 + ms);
        return (int)((time - baseDate).TotalMilliseconds);
    }

    public override int setHours(int timeMs, int hour, int minute, int second, int ms)
    {
        DateTime time = new DateTime(timeMs * 10000).AddMilliseconds(hour * 3600000 + minute * 60000 + second * 1000 + ms);
        return (int)((time - baseDate).TotalMilliseconds);
    }

    public void setPortrait(IProfileInput input, string portrait)
    {
        input.profile.portrait = portrait;
    }
}