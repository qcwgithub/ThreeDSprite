using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GenPlayerToSqlTable
{
    public static string Do(List<ProfileConfig> configs)
    {
        var f = new FileFormatter();

        f.AddTab(3);
        for (int i = 0; i < configs.Count; i++)
        {
            ProfileConfig config = configs[i];
            if (config.dataManagement == DataManagement.server ||
                config.dataManagement == DataManagement.server_client)
            {
                f.PushTab();
                f.Push("sql.", config.field, " = ", config.type.PlayerToSqlTable("profile", config.field));
                f.Push(";").PushLine();
            }
        }

        string str = f.GetString();
        return str;
    }
}