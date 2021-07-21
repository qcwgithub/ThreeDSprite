using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GenPMScriptCreateNewPlayer
{
    public static string Do(List<ProfileConfig> configs)
    {
        var f = new FileFormatter();

        f.AddTab(3);
        // char alpha = (char)0;
        for (int i = 0; i < configs.Count; i++)
        {
            ProfileConfig config = configs[i];
            f.PushTab().Push(string.Format("profile.{0} = {1};", config.field, 
                !string.IsNullOrEmpty(config.defaultValueExp) ? config.defaultValueExp : config.type.defaultValueExp())).PushLine();
        }

        string str = f.GetString();
        return str;
    }
}