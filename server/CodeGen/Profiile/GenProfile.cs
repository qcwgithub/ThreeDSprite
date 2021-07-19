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
            // if (alpha == (char)0 || alpha != config.field[0])
            // {
            //     if (alpha != (char)0)
            //     {
            //         f.PushLine();
            //     }
            //     f.PushTab();
            //     alpha = config.field[0];
            //     f.Push("//// ", alpha.ToString());
            //     f.PushLine();
            // }

            f.PushTab().Push("[Key(", i.ToString(), ")]").PushLine();

            f.PushTab();
            if (config.dataManagement == DataManagement.delete)
            {
                f.Push("private ");
            }

            f.Push("public ", config.type.ProfileTypeString(), " ", config.field, ";");
            f.PushLine();
        }

        string str = f.GetString();
        return str;
    }
}