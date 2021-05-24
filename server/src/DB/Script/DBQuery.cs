using System;
using System.Collections;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

public class DBQuery : DBHandler
{
    public override MsgType msgType { get { return MsgType.DBQuery; } }

    public override async Task<MyResponse> handle(object socket, string _msg)
    {
        var msg = this.baseScript.castMsg<MsgDBQuery>(_msg);
        this.logger.debug("DBQuery: " + msg.queryStr);
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

        var sqlParams = new MySqlParameter[msg.values.Count];
        for (int i = 0; i < msg.values.Count; i++)
        {
            sqlParams[i] = new MySqlParameter("@" + i, msg.values[i]);
        }

        switch (operation)
        {
            case "UPDATE":
            case "INSERT":
                MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, msg.queryStr, sqlParams);
                break;
            case "SELECT":
                MySqlHelper.ExecuteReaderAsync(this.dbData.connectionString, msg.queryStr, sqlParams);
                break;
            default:
                return ECode.InvalidParam;
        }

        var data = this.dbData;
        var waiter = new WaitCallBack();
        
        Action action =() =>
        {
            var cb = (error: mysql.MysqlError, object result/*, object fields*/) =>
            {
                if (error)
                {
                    this.baseScript.error("DBQuery error: " + this.server.JSON.stringify(error));
                    var res = new ResMysqlError { code = error.code, errno = error.errno };
                    waiter.finish(new MyResponse(ECode.SqlError, res));
                }
                else
                {
                    if (msg.expectedAffectedRows >= 0)
                    {
                        if (result == null)
                        {
                            this.baseScript.error("expectedAffectedRows: %d, result==null, queryStr: %s", msg.expectedAffectedRows, msg.queryStr);
                        }
                        else
                        {
                            var pkt = result as mysql.OkPacket;
                            if (pkt.affectedRows != msg.expectedAffectedRows)
                            {
                                this.baseScript.error("expectedAffectedRows: %d, okPackage.affectedRows=%s, queryStr: %s",
                                    msg.expectedAffectedRows,
                                    (pkt.affectedRows == null ? "null" : pkt.affectedRows.toString()),
                                    msg.queryStr);
                            }
                        }
                    }
                    waiter.finish(new MyResponse(ECode.Success, result));
                }
            };

            if (msg.values == null)
            {
                data.pool.query(msg.queryStr, cb);
            }
            else
            {
                data.pool.query(msg.queryStr, msg.values, cb);
            }
        };

        return await waiter.Task;
    }
}