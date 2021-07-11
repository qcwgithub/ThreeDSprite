using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class MessageCodeEditor
{

    [MenuItem("TDS/Gen Message Code")]
    static void GenMessageCode_()
    {
        GenMessageCode();
        GenBinaryMessagePackerGen();
    }

    readonly static string Header1 = @"using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;

namespace Script
{
    public partial class BinaryMessagePacker : MessageHeaderPacker
    {
        
";
    readonly static string Tail1 = @"    }
}";
    static void GenBinaryMessagePackerGen()
    {
        var sb = new StringBuilder();
        sb.Append(Header1);

        sb.Append("        byte[] DoPack<T>(T msg)").Append("\n");
        sb.Append("        {").Append("\n");
        sb.Append("            MessageCode code = TypeToMessageCodeCache<T>.code;").Append("\n");
        sb.Append("            byte[] bytes = MessagePackSerializer.Serialize<T>(msg);").Append("\n");
        sb.Append("            int totalLength = this.GetHeaderLength() + sizeof(int) + bytes.Length").Append("\n");
        sb.Append("            var buffer = new byte[totalLength];");
        sb.Append("        }").Append("\n");

        sb.Append(Tail1);

        string file = "Assets/Scripts/SCCommonScript/BinaryMessagePackerGen.cs";
        File.WriteAllText(file, sb.ToString());
        AssetDatabase.Refresh();

        Debug.Log("Done. (Click to locate BinaryMessagePackerGen.cs)", AssetDatabase.LoadMainAssetAtPath(file));
        // MessagePack.MessagePackSerializer.Serialize<Data.BMResMove>()
    }

    ////////////////////////////////////////////////////////////////////

    readonly static string Header = @"namespace Data
{
    public enum MessageCode
    {
        #region AUTO
";

    readonly static string Tail = @"        #endregion
    }
}";
    static void GenMessageCode()
    {
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
    }

    ////////////////////////////////////////////////////////////////////
}
