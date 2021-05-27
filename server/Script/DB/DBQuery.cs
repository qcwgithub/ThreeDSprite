using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Data;

public class DBQuery : DBHandler
{
    public override MsgType msgType { get { return MsgType.DBQuery; } }

    public override async Task<MyResponse> handle(ISocket socket, string _msg)
    {
        var msg = this.baseScript.decodeMsg<MsgDBQuery>(_msg);
        this.logger.Debug("DBQuery: " + msg.queryStr);
        // find operation
        int spaceIndex = msg.queryStr.IndexOf(' ');
        if (spaceIndex < 0)
        {
            return ECode.InvalidParam;
        }
        string operation = msg.queryStr.Substring(0, spaceIndex).ToUpper();

        this.dbData.queryCount++;

        if (msg.valueTypes != null)
        {
            foreach (var kv in msg.valueTypes)
            {
                var index = kv.Key;
                if (!(index >= 0 && index < msg.values.Count))
                {
                    return ECode.DBQueryValueTypesError;
                }

                MyDBValueType type = (MyDBValueType)kv.Value;
                switch (type)
                {
                    case MyDBValueType.DateTime:
                        {
                            long ticks = TimeSpan.TicksPerMillisecond * (long)(int)msg.values[index];
                            msg.values[index] = new DateTime(ticks);
                        }
                        break;
                    default:
                        return ECode.DBQueryValueTypesError;
                }
            }
        }

        int valueCount = msg.values == null ? 0 : msg.values.Count;
        var sqlParams = new MySqlParameter[valueCount];
        for (int i = 0; i < valueCount; i++)
        {
            sqlParams[i] = new MySqlParameter("@" + i, msg.values[i]);
        }

        int affectedRows = 0;
        object res = null;
        switch (operation)
        {
            case "UPDATE":
            case "INSERT":
                {
                    affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, msg.queryStr, sqlParams);
                }
                break;
            case "SELECT":
                {
                    MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(this.dbData.connectionString, msg.queryStr, sqlParams);
                    var dict = new Dictionary<string, List<object>>();
                    while (await reader.ReadAsync())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string key = reader.GetName(i);
                            object value = reader.GetValue(i);
                            List<object> list;
                            if (!dict.TryGetValue(key, out list))
                            {
                                list = new List<object>();
                                dict.Add(key, list);
                            }
                            list.Add(value is DBNull ? null : value);
                        }
                    }
                    await reader.CloseAsync();
                    affectedRows = reader.RecordsAffected;
                    res = dict;
                }
                break;
            default:
                return ECode.InvalidParam;
        }

        if (msg.expectedAffectedRows >= 0)
        {
            if (msg.expectedAffectedRows != affectedRows)
            {
                this.server.logger.ErrorFormat("expectedAffectedRows: {0} != affectedRows={1}, queryStr: {2}",
                    msg.expectedAffectedRows, affectedRows, msg.queryStr);
            }
        }

        return new MyResponse(ECode.Success, res);
    }
}