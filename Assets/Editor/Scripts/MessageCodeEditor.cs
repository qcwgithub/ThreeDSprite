using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Data;

public class MessageCodeEditor
{
    [MenuItem("TDS/Gen Message Code")]
    static void GenMessageCode_()
    {
        var list = GenMessageCode();
        GenBinaryMessagePackerGen(list);
    }

    ////////////////////////////////////////////////////////////////////
    #region MessageCode.cs

    readonly static string Header = @"namespace Data
{
    public enum MessageCode
    {
        #region AUTO
";

    readonly static string Tail = @"        #endregion
    }
}";
    static List<string> GenMessageCode()
    {
        var list = new List<string>();
        var sb = new StringBuilder();
        sb.Append(Header);

        var allDataTypes = typeof(Data.MsgType).Assembly.GetTypes();
        foreach (var type in allDataTypes)
        {
            var attrs = type.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                if (attr is MessagePack.MessagePackObjectAttribute)
                {
                    list.Add(type.Name);
                    sb.Append("        " + type.Name).Append(",\n");

                    break;
                }
            }
        }

        sb.Append(Tail);

        string file = "Assets/Scripts/SCCommonData/MessageCode.cs";
        File.WriteAllText(file, sb.ToString());
        AssetDatabase.Refresh();

        Debug.Log("Done. (Click to locate MessageCode.cs)", AssetDatabase.LoadMainAssetAtPath(file));
        return list;
    }

    #endregion
    ////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////
    #region BinaryMessagePacker.cs

    readonly static string Header1 = @"using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;

namespace Script
{
    public partial class BinaryMessagePacker
    {
        
";
    readonly static string Tail1 = @"    }
}";
    static void GenBinaryMessagePackerGen(List<string> list)
    {
        var sb = new StringBuilder();
        sb.Append(Header1);

        sb.Append("        object UnpackBody(MessageCode messageCode, byte[] buffer, int offset, int count)").Append("\n");
        sb.Append("        {").Append("\n");
        
        sb.Append("            object obj = null;").Append("\n");
        sb.Append("            var readonlyMemory = new ReadOnlyMemory<byte>(buffer, offset, count);").Append("\n");

        sb.Append("            switch (messageCode)").Append("\n");
        sb.Append("            {").Append("\n");

        foreach (var e in list)
        {
            sb.AppendFormat("                case MessageCode.{0}: obj = MessagePackSerializer.Deserialize<{0}>(readonlyMemory); break;", e).Append("\n");
        }
        sb.Append("                default:").Append("\n");
        sb.Append("                    throw new Exception();").Append("\n");
        sb.Append("                    //break;").Append("\n");
        sb.Append("            }").Append("\n");

        sb.Append("            return obj;").Append("\n");

        sb.Append("        }").Append("\n");

        sb.Append(Tail1);

        string file = "Assets/Scripts/SCCommonScript/BinaryMessagePackerGen.cs";
        File.WriteAllText(file, sb.ToString());
        AssetDatabase.Refresh();

        Debug.Log("Done. (Click to locate BinaryMessagePackerGen.cs)", AssetDatabase.LoadMainAssetAtPath(file));
        // MessagePack.MessagePackSerializer.Deserialize

    }

    #endregion
    ////////////////////////////////////////////////////////////////////
}
