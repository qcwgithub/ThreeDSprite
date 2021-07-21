using System;
using System.Collections.Generic;
using System.IO;

public class Mark
{
    public string startMark;
    public string text;
}

public class ProfileProgram
{
    static string ReplaceText(string content, string startMark, string endMark, string text)
    {
        int start = content.IndexOf(startMark);
        if (start < 0)
        {
            Console.WriteLine("startMark " + startMark + " not found");
            throw new Exception();
        }
        start = content.IndexOf('\n', start + 1);

        int end = content.IndexOf(endMark);
        if (end <= 0)
        {
            Console.WriteLine("endMark " + endMark + " not found");
            throw new Exception();
        }
        end = content.LastIndexOf('\n', end - 1);

        string pre = content.Substring(0, start + 1) + "\n";
        string post = content.Substring(end);
        return pre + text + post;
    }

    static void ReplaceFile(string file, Mark[] array)
    {
        string content = File.ReadAllText(file);
        for (int i = 0; i < array.Length; i++)
        {
            string endMark = array[i].startMark.Replace("#region", "#endregion");
            content = ReplaceText(content, array[i].startMark, endMark, array[i].text);
        }
        File.WriteAllText(file, content);
    }

    public static void Do()
    {
        List<ProfileConfig> configs = new List<ProfileConfig>();
        Script.CsvHelper helper = Script.CsvUtils.Parse(File.ReadAllText("server/CodeGen/ProfileConfig.csv"));
        while (helper.ReadRow())
        {
            ProfileConfig c = new ProfileConfig();
            c.comment = helper.ReadString("comment");
            c.field = helper.ReadString("field");
            c.type = helper.ReadEnum<FieldType>("type");
            c.dataManagement = helper.ReadEnum<DataManagement>("dataManagement");
            c.defaultValueExp = helper.ReadString("defaultValueExp");
            c.sql = helper.ReadString("sql");

            configs.Add(c);
        }

        ReplaceFile("server/Data/Common/SCCommonData/Profile.cs", new Mark[]
        {
            new Mark { startMark = "#region Profile Auto", text = GenProfile.Do(configs) },
            // new Mark { startMark = "//#region autoEnsures", text = "" },
        });

        ReplaceFile("server/Data/Common/SqlTablePlayer.cs", new Mark[]
        {
            new Mark { startMark = "#region SqlTablePlayer Auto", text = GenSqlTablePlayer.Do(configs) }
        });

        ReplaceFile("server/Data/Common/messagesS.cs", new Mark[]
        {
            new Mark { startMark = "#region MsgSavePlayer Auto", text = GenMsgSavePlayer.Do(configs) }
        });

        ReplaceFile("server/Script/PM/PMPlayerToSqlTable.cs", new Mark[]
        {
            new Mark { startMark = "#region PMPlayerToSqlTable Auto", text = GenPlayerToSqlTable.Do(configs) }
        });

        ReplaceFile("server/Script/PM/PMSqlTableToPlayer.cs", new Mark[]
        {
            new Mark { startMark = "#region PMSqlTableToPlayer Auto", text = GenSqlTableToPlayer.Do(configs) }
        });

        ReplaceFile("server/Script/PM/PMPlayerSave.cs", new Mark[]
        {
            new Mark { startMark = "#region PMPlayerSave Auto", text = GenPMPlayerSave.Do(configs) }
        });
        
        ReplaceFile("server/Script/PM/PMScriptCreateNewPlayer.cs", new Mark[]
        {
            new Mark { startMark = "#region PMScriptCreateNewPlayer Auto", text = GenPMScriptCreateNewPlayer.Do(configs) }
        });

        ReplaceFile("server/Script/DB/DBSavePlayer.cs", new Mark[]
        {
            new Mark { startMark = "#region DBSavePlayer Auto", text = GenDBSavePlayer.Do(configs) }
        });

        ReplaceFile("server/Script/DB/DBInsertPlayer.cs", new Mark[]
        {
            new Mark { startMark = "#region DBInsertPlayer Auto", text = GenDBInsertPlayer.Do(configs) }
        });

        ReplaceFile("server/Script/DB/DBQueryPlayer.cs", new Mark[]
        {
            new Mark { startMark = "#region DBQueryPlayer Auto", text = GenDBQueryPlayer.Do(configs) }
        });

        ReplaceFile("server/sql/player.sql", new Mark[]
        {
            new Mark { startMark = "#region player.sql Auto", text = GenPlayerSql.Do(configs) }
        });
    }
}