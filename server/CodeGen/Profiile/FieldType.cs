using System;
public enum FieldType
{
    int_,
    string_,
    BigInteger,
}

public static class FieldTypeExt
{
    public static string ToProfile(this FieldType e)
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

    public static string ToSqlTablePlayer(this FieldType e)
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

    public static string ToMsgSavePlayer(this FieldType e)
    {
        switch (e)
        {
            case FieldType.int_: return "int?";
            case FieldType.string_: return "string";
            case FieldType.BigInteger: return "BigInteger";
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
            case FieldType.BigInteger:
                return string.Format("{0}.{1}", objName, field);

                // return string.Format("BigInteger.Parse({0}.{1})", objName, field);

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
            case FieldType.BigInteger:
                return string.Format("{0}.{1}", objName, field);

                // return string.Format("{0}.{1}.ToString()", objName, field);

            default:
                throw new Exception("unknown field type");
        }
    }

    public static string FromDB(this FieldType e, string objName, string field)
    {
        switch (e)
        {
            case FieldType.int_:
                return string.Format("{0}.GetInt32(\"{1}\")", objName, field);
            
            case FieldType.string_:
                return string.Format("{0}.GetString(\"{1}\")", objName, field);

            case FieldType.BigInteger:
                return string.Format("BigInteger.Parse({0}.GetString(\"{1}\"))", objName, field);

            default:
                throw new Exception("unknown field type");
        }
    }

    public static string ToDB(this FieldType e, string objName, string field)
    {
        switch (e)
        {
            case FieldType.int_:
            case FieldType.string_:
                return string.Format("{0}.{1}", objName, field);
            
                // return string.Format("{0}.GetString(\"{1}\")", objName, field);

            case FieldType.BigInteger:
                return string.Format("{0}.{1}.ToString()", objName, field);

            default:
                throw new Exception("unknown field type");
        }
    }

    public static string defaultValueExp(this FieldType e)
    {
        switch (e)
        {
            case FieldType.string_:
                return "\"\"";
            
                // return string.Format("{0}.GetString(\"{1}\")", objName, field);

            case FieldType.int_:
            case FieldType.BigInteger:
            default:
                throw new Exception("unknown field type");
        }
    }
}