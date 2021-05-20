using System.Collections.Generic;

public class PayIvySqlUtils : IScript {
    public Server server { get; set; }
    // PM
    public IEnumerator insertPayIvyYield(int playerId, int id, string productId, int quantity, string fen, string orderId, MyResponse r) {
        var queryStr = "INSERT INTO payivy (playerId,id,productId,quantity,fen,orderId,state,createTime,notifyTime,gotTime) VALUES (?,?,?,?,?,?,?,?,?,?)";
        var currTime = new Date().getTime();
        // var defaultTime = this.server.baseData.defaultDateTime;
        object defaultTime = null;
        object[] values = new object[] { playerId, id, productId, quantity, fen, orderId, PayIvyState.Init, currTime/* index=L-3 */, defaultTime/* index=L-2 */, defaultTime/* index=L-1 */ };
        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[values.Length - 3] = (int)MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 2] = MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 1] = MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
        return r;
    }

    // AAA
    // select 时不用 detail，有点大，省一点
    private string SELECT_ROWS = "orderId,playerId,id,productId,quantity,fen,state,got,createTime,notifyTime,gotTime,orderId3";
    *queryPayIvy_orderId(string orderId) {
        var queryStr = string.Format("SELECT {0} FROM payivy WHERE orderId=?", this.SELECT_ROWS);
        object[] values = new object[] { orderId };
        MsgDBQuery msg = new MsgDBQuery { queryStr = queryStr, values = values };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
        if (r.err != ECode.Success) {
            return r.err;
        }
        var info = this.decodePayIvy(r.res);
        return new MyResponse(ECode.Success, info);
    }

    *queryPayIvy_p_s_g(int playerId, int state, int got) {
        string queryStr = string.Format("SELECT {0} FROM payivy WHERE playerId=? AND state=? AND got=?", this.SELECT_ROWS);

        object[] values = new object[]  { playerId, state, got };
        MsgDBQuery msg = new MsgDBQuery { queryStr = queryStr, values = values };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
        if (r.err != ECode.Success) {
            return r.err;
        }
        SqlTablePayIvy[] infos = this.decodePayIvyMany(r.res);
        return new MyResponse(ECode.Success, infos);
    }

    *updatePayIvyGotMany(string[] orderIds, int got) {
        var queryStr = "UPDATE payivy SET got=?,gotTime=? WHERE orderId IN (";
        for (int i = 0; i < orderIds.Length; i++) {
            queryStr += "?";
            if (i < orderIds.Length - 1) {
                queryStr += ",";
            }
        }
        queryStr += ");";

        List<object> values = new List<object> { got, new Date().getTime()/* index=1 */ };
        for (int i = 0; i < orderIds.Length; i++) {
            values.Add(orderIds[i]);
        }

        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = orderIds.Length,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[1] = (int)MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
        return r;
    }

    // AAA
    // 根据 orderId 更新 state 和 orderId3, notifyTime
    public IEnumerator updatePayIvyStateYield(string orderId, int state, string orderId3, string detail, MyResponse r) {
        var queryStr = "UPDATE payivy SET state=?,orderId3=?,notifyTime=?,detail=? WHERE orderId=?";
        List<object> values = new List<object> { state, orderId3, new Date().getTime()/* index=2 */, detail, orderId };
        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[2] = (int)MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, r);
        return r;
    }

    private SqlTablePayIvy decodePayIvy(SqlTablePayIvy[] sqlDatas) {
        if (sqlDatas == null || sqlDatas.Length != 1) {
            return null;
        }
        var sqlData = sqlDatas[0];
        SqlTablePayIvy info = sqlData;
        return info;
    }
    private List<SqlTablePayIvy> decodePayIvyMany(SqlTablePayIvy[] sqlDatas) {
        if (sqlDatas == null) {
            return null;
        }

        List<SqlTablePayIvy> array = new List<SqlTablePayIvy>();
        for (int i = 0; i < sqlDatas.Length; i++) {
            var sqlData = sqlDatas[i];
            SqlTablePayIvy info = sqlData;
            array.Add(info);
        }
        return array;
    }
}