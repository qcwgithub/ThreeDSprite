using UnityEngine;

public class LSKeys
{
    public const string CHANNEL = ProjectUtils.ProjectName + "Channel";
    public const string PC_CHANNEL_USER_ID = ProjectUtils.ProjectName + "PcChannelUserId";
    public const string UUID_CHANNEL_USER_ID = ProjectUtils.ProjectName + "UuidChannelUserId";
    public const string APPLE_CHANNEL_USER_ID = ProjectUtils.ProjectName + "AppWSServerAddress";
}

public class LSUtils
{
    public static string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }
    
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
