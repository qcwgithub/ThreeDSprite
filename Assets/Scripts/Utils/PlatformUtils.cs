using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformUtils
{
    public static string getPlatformString()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            default:
                return "pc";
        }
    }

    public static string getAppVersion()
    {
        return Application.version;
    }
}
