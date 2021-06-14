using System;
using UnityEngine;
public class TimeMgr
{
    public static DateTime GMT = new DateTime(1970, 1, 1, 0, 0, 0);
    public static long SecondBase = 10000000L;
    public static long MillisecondBase = 10000;
    public static int OneHourSecond = 3600;
    public static int OneDayHour = 24;

    private double startTimeS;
    private int timezoneOffset;
    private static TimeMgr instance = new TimeMgr();
    public static TimeMgr Instance { get { return instance; } }
    public bool isTrustedTime;

    private TimeMgr()
    {
        isTrustedTime = false;
        startTimeS = (DateTime.Now - TimeMgr.GMT).TotalSeconds - Time.realtimeSinceStartup;
        timezoneOffset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
    }

    public void Init(double timeS)
    {
        isTrustedTime = true;
        startTimeS = timeS;
    }

    public int NowTimeS { get { return (int)(startTimeS + Time.realtimeSinceStartup); } }
    public long NowTimeMS { get { return (long)(startTimeS + Time.realtimeSinceStartup) * 1000L; } }

    public void SetServerTime(long timeMs, int timezoneOffset)
    {
        isTrustedTime = true;
        startTimeS = timeMs / 1000 - Time.realtimeSinceStartup;
        this.timezoneOffset = timezoneOffset;
    }
}