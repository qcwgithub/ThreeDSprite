using System;
using System.Collections;
using System.Collections.Generic;

public class PMSqlUtils : SqlUtils
{
    public IEnumerator selectPlayerYield(int playerId, MyResponse r)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "SELECT * FROM player WHERE id=?;",
            values = new List<object> { playerId }
        };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
    }

    private string createInsertQueryStr(PMPlayerInfo player, List<string> fields, List<object> values)
    {
        var L = fields.Count;
        if (L == 0)
        {
            return null;
        }
        if (L != values.Count)
        {
            this.server.baseScript.error("createInsertQueryStr fields.length != values.length");
            return null;
        }

        List<string> buffer = new List<string>();
        buffer.Add("INSERT INTO player (id,");//) VALUES (" + player.id + ",");
        for (int i = 0; i < L; i++)
        {
            buffer.Add(fields[i]);
            if (i < L - 1)
            {
                buffer.Add(",");
            }
        }
        buffer.Add(") VALUES (" + player.id + ",");
        for (int i = 0; i < L; i++)
        {
            buffer.Add("?");
            if (i < L - 1)
            {
                buffer.Add(",");
            }
        }
        buffer.Add(")");

        var queryStr = string.Join('', buffer.ToArray());
        return queryStr;
    }

    private string createUpdateQueryStr(PMPlayerInfo player, List<string> fields, List<object> values)
    {
        var L = fields.Count;
        if (L == 0)
        {
            return null;
        }
        if (L != values.Count)
        {
            this.server.baseScript.error("saveFieldBatch fields.length != values.length");
            return null;
        }

        List<string> buffer = new List<string>();
        buffer.Add("UPDATE player SET ");
        for (int i = 0; i < L; i++)
        {
            buffer.Add(fields[i] + " = ?");
            if (i < L - 1)
            {
                buffer.Add(",");
            }
        }
        buffer.Add(" WHERE id=" + player.id);

        var queryStr = string.Join('', buffer.ToArray());
        return queryStr;
    }

    private void saveFieldBatch(PMPlayerInfo player, List<string> fields, List<object> values)
    {
        var queryStr = this.createUpdateQueryStr(player, fields, values);
        if (queryStr == null)
        {
            return;
        }
        var msg = new MsgDBQuery
        {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        this.server.netProto.send(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, (MyResponse r) =>
        {
            if (r.err != ECode.Success)
            {
                this.server.baseScript.error("saveFieldBatch failed. " + queryStr);
            }
        });
    }

    public IEnumerator saveFieldBatchYield(PMPlayerInfo player, List<string> fields, List<object> values, MyResponse r)
    {
        var queryStr = this.createUpdateQueryStr(player, fields, values);
        if (queryStr == null)
        {
            r.err = ECode.Error;
            yield break;
        }

        var msg = new MsgDBQuery
        {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
    }

    // 仅用于新玩家
    public IEnumerator insertPlayerYield(PMPlayerInfo player, MyResponse r)
    {
        var obj = new PMSqlHelpObject();
        obj.player = player;
        obj.fields = new List<string>();
        obj.values = new List<object>();

        //#region autoInsertPlayer >>>>>>>>自动生成区域开始

        //#endregion autoInsertPlayer <<<<<<<<自动生成区域结束

        // 注意索引被后面使用
        obj.fields.Add("createTime");
        obj.values.Add(this.server.gameScript.getTime());
        var valueTypes = new Dictionary<int, int>();
        valueTypes[obj.fields.Count - 1] = (int)MyDBValueType.DateTime;

        var queryStr = this.createInsertQueryStr(player, obj.fields, obj.values);
        if (queryStr == null)
        {
            r.err = ECode.Error;
            yield break;
        }

        var msg = new MsgDBQuery
        {
            queryStr = queryStr,
            values = obj.values,
            valueTypes = valueTypes,
            expectedAffectedRows = 1,
        };

        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
    }

    public IEnumerator insertPayiOSYield(int playerId, string env, int id, string productId, string bundleId, int quantity, string transactionId, string originalTransactionId, int purchaseDateMs, int expiresDateMs, MyResponse r)
    {
        var queryStr = "INSERT INTO payios (playerId,env,id,productId,bundleId,quantity,transactionId,originalTransactionId,purchaseDate,expiresDate) VALUES (?,?,?,?,?,?,?,?,?,?)";
        List<object> values = new List<object> { playerId, env, id, productId, bundleId, quantity, transactionId, originalTransactionId, purchaseDateMs, expiresDateMs };
        var msg = new MsgDBQuery
        {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[values.Count - 2] = (int)MyDBValueType.DateTime;
        msg.valueTypes[values.Count - 1] = (int)MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
    }

    private PMSqlHelpObject newHelpObject(PMPlayerInfo player)
    {
        var obj = new PMSqlHelpObject();
        obj.player = player;
        obj.fields = new List<string>();
        obj.values = new List<object>();
        return obj;
    }

    public void save(PMPlayerInfo player, Action<PMSqlHelpObject> fun)
    {
        var obj = this.newHelpObject(player);
        fun(obj);
        this.saveFieldBatch(player, obj.fields, obj.values);
    }

    public IEnumerator saveYield(PMPlayerInfo player, Action<PMSqlHelpObject> fun, MyResponse r)
    {
        var obj = this.newHelpObject(player);
        fun(obj);
        yield return this.saveFieldBatchYield(player, obj.fields, obj.values, r);
    }

    public PMSqlHelpObject beginSave(PMPlayerInfo player)
    {
        var obj = this.newHelpObject(player);
        return obj;
    }
    public void endSave(PMSqlHelpObject obj)
    {
        this.saveFieldBatch(obj.player, obj.fields, obj.values);
    }

    public IEnumerator endSaveYield(PMSqlHelpObject obj, MyResponse r)
    {
        yield return this.saveFieldBatchYield(obj.player, obj.fields, obj.values, r);
    }
}