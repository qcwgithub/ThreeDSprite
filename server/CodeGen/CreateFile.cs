using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CreateFile
{
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
    public static void GenMessageCode(List<string> list, string outputFile)
    {
        var sb = new StringBuilder();
        sb.Append(Header);

        foreach (var name in list)
        {
            sb.Append("        " + name).Append(",\n");
        }

        sb.Append(Tail);

        File.WriteAllText(outputFile, sb.ToString());
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
    public static void GenBinaryMessagePackerGen_Unpack(List<string> list, string outputFile, StringBuilder sb)
    {
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

    }

    #endregion
    ////////////////////////////////////////////////////////////////////

    
    ////////////////////////////////////////////////////////////////////
    #region BinaryMessagePacker.cs
    
    public static void GenBinaryMessagePackerGen_Pack(List<string> list, string outputFile, StringBuilder sb)
    {

        sb.Append("        byte[] PackBody(MessageCode messageCode, object obj)").Append("\n");
        sb.Append("        {").Append("\n");

        sb.Append("            byte[] bytes = null;").Append("\n");

        sb.Append("            switch (messageCode)").Append("\n");
        sb.Append("            {").Append("\n");

        foreach (var e in list)
        {
            sb.AppendFormat("                case MessageCode.{0}: bytes = MessagePackSerializer.Serialize<{0}>(({0})obj); break;", e).Append("\n");
        }
        sb.Append("                default:").Append("\n");
        sb.Append("                    throw new Exception();").Append("\n");
        sb.Append("                    //break;").Append("\n");
        sb.Append("            }").Append("\n");

        sb.Append("            return bytes;").Append("\n");

        sb.Append("        }").Append("\n");

    }

    #endregion
    ////////////////////////////////////////////////////////////////////

    public static void GenBinaryMessagePackerGen(List<string> list, string outputFile)
    {
        var sb = new StringBuilder();
        sb.Append(Header1);

        GenBinaryMessagePackerGen_Pack(list, outputFile, sb);
        GenBinaryMessagePackerGen_Unpack(list, outputFile, sb);

        sb.Append(Tail1);
        
        File.WriteAllText(outputFile, sb.ToString());
    }
}
