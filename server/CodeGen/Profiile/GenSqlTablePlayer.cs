using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GenSqlTablePlayer
{
    public static string Do(List<ProfileConfig> configs)
    {
        var f = new FileFormatter();

        f.AddTab(2);
        // char alpha = (char)0;
        for (int i = 0; i < configs.Count; i++)
        {
            ProfileConfig config = configs[i];
            if (config.dataManagement == DataManagement.server ||
                config.dataManagement == DataManagement.server_client)
            {
                f.PushTab().Push("[Key(", (i + 1).ToString(), ")]").PushLine();
                f.PushTab().Push("public ", config.type.SqlTablePlayerTypeString());
                f.Push(" ", config.field, ";").PushLine();
            }
        }

        string str = f.GetString();
        return str;
    }
}