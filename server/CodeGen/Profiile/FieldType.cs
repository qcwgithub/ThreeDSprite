using System;
public enum FieldType
{
    int_,
    string_,
    BigInteger,
}

public static class FieldTypeExt
{
    public static string ProfileTypeString(this FieldType e)
    {
        switch (e)
        {
            case FieldType.int_: return "int";
            case FieldType.string_: return "string";
            case FieldType.BigInteger: return "BigInteger";
            default:
                throw new Exception("unknown field type");
        }
    }

    public static string SqlTablePlayerTypeString(this FieldType e)
    {
        switch (e)
        {
            case FieldType.int_: return "int";
            case FieldType.string_: return "string";
            case FieldType.BigInteger: return "string";
            default:
                throw new Exception("unknown field type");
        }
    }

    public static string MsgSavePlayerString(this FieldType e)
    {
        switch (e)
        {
            case FieldType.int_: return "int?";
            case FieldType.string_: return "string";
            case FieldType.BigInteger: return "string";
            default:
                throw new Exception("unknown field type");
        }
    }

    public static string SqlTableToPlayer(this FieldType e, string objName, string field)
    {
        switch (e)
        {
            case FieldType.int_:
            case FieldType.string_:
                return string.Format("{0}.{1}", objName, field);

            case FieldType.BigInteger:
                return string.Format("BigInteger.Parse({0}.{1})", objName, field);

            default:
                throw new Exception("unknown field type");
        }
    }

    public static string PlayerToSqlTable(this FieldType e, string objName, string field)
    {
        switch (e)
        {
            case FieldType.int_:
            case FieldType.string_:
                return string.Format("{0}.{1}", objName, field);

            case FieldType.BigInteger:
                return string.Format("{0}.{1}.ToString()", objName, field);

            default:
                throw new Exception("unknown field type");
        }
    }
}