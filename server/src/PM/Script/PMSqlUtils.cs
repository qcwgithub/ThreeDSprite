using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PMSqlUtils : SqlUtils
{
    public async Task<MyResponse> selectPlayerYield(int playerId)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "SELECT * FROM player WHERE id=@0;",
            values = new List<object> { playerId }
        };
        return await this.server.baseData.dbPlayerSocket.sendAsync(MsgType.DBQuery, msg);
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
            buffer.Add("@" + i);
            if (i < L - 1)
            {
                buffer.Add(",");
            }
        }
        buffer.Add(")");

        var queryStr = string.Join(null, buffer.ToArray());
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
            buffer.Add(fields[i] + "=@" + i);
            if (i < L - 1)
            {
                buffer.Add(",");
            }
        }
        buffer.Add(" WHERE id=" + player.id);

        var queryStr = string.Join(null, buffer.ToArray());
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
        this.server.baseData.dbPlayerSocket.send(MsgType.DBQuery, msg, (e, r) =>
        {
            if (e != ECode.Success)
            {
                this.server.baseScript.error("saveFieldBatch failed. " + queryStr);
            }
        });
    }

    public async Task<MyResponse> saveFieldBatchYield(PMPlayerInfo player, List<string> fields, List<object> values)
    {
        var queryStr = this.createUpdateQueryStr(player, fields, values);
        if (queryStr == null)
        {
            return ECode.Error;
        }

        var msg = new MsgDBQuery
        {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        return await this.server.baseData.dbPlayerSocket.sendAsync(MsgType.DBQuery, msg);
    }

    // 仅用于新玩家
    public async Task<MyResponse> insertPlayerYield(PMPlayerInfo player)
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
            return ECode.Error;
        }

        var msg = new MsgDBQuery
        {
            queryStr = queryStr,
            values = obj.values,
            valueTypes = valueTypes,
            expectedAffectedRows = 1,
        };

        return await this.server.baseData.dbPlayerSocket.sendAsync(MsgType.DBQuery, msg);
    }

    public async Task<MyResponse> insertPayiOSYield(int playerId, string env, int id, string productId, string bundleId, int quantity, string transactionId, string originalTransactionId, int purchaseDateMs, int expiresDateMs)
    {
        var queryStr = "INSERT INTO payios (playerId,env,id,productId,bundleId,quantity,transactionId,originalTransactionId,purchaseDate,expiresDate) VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9)";
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
        return await this.server.baseData.dbPlayerSocket.sendAsync(MsgType.DBQuery, msg);
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

    public async Task<MyResponse> saveYield(PMPlayerInfo player, Action<PMSqlHelpObject> fun)
    {
        var obj = this.newHelpObject(player);
        fun(obj);
        return await this.saveFieldBatchYield(player, obj.fields, obj.values);
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

    public async Task<MyResponse> endSaveYield(PMSqlHelpObject obj)
    {
        return await this.saveFieldBatchYield(obj.player, obj.fields, obj.values);
    }
}