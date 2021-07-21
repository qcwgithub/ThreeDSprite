using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GenProfile
{
    public static string Do(List<ProfileConfig> configs)
    {
        var f = new FileFormatter();

        f.AddTab(2);
        // char alpha = (char)0;
        for (int i = 0; i < configs.Count; i++)
        {
            ProfileConfig config = configs[i];
            f.PushTab().Push(string.Format("[Key({0})]", i)).PushLine();
            f.PushTab().Push(string.Format("public {0} {1};", config.type.ProfileTypeString(), config.field)).PushLine();
        }

        string str = f.GetString();
        return str;
    }
}