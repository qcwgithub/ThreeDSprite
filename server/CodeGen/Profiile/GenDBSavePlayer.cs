using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GenDBSavePlayer
{
    public static string Do(List<ProfileConfig> configs)
    {
        var f = new FileFormatter();

        f.AddTab(3);
        // char alpha = (char)0;
        for (int i = 0; i < configs.Count; i++)
        {
            ProfileConfig config = configs[i];
            if (config.dataManagement == DataManagement.server ||
                config.dataManagement == DataManagement.server_client)
            {
                f.PushTab().Push(string.Format("if (msg.{0} != null)", config.field)).PushLine();
                f.PushTab().Push("{").PushLine();
                f.AddTab(1);
                f.PushTab().Push(string.Format("fields.Add(\"{0}=@\" + values.Count);", config.field)).PushLine();
                f.PushTab().Push(string.Format("values.Add(msg.{0});", config.field)).PushLine();
                f.AddTab(-1);
                f.PushTab().Push("}").PushLine();
                f.PushLine();
            }
        }

        string str = f.GetString();
        return str;
    }
}