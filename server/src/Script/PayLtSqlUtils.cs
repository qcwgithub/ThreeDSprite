using System.Collections.Generic;

public class PayLtSqlUtils : IScript {
    public Server server { get; set; }
    // PM
    *insertPayLtYield(int playerId, int id, string productId, int quantity, string fen, string orderId) {
        var queryStr = "INSERT INTO paylt (playerId,id,productId,quantity,fen,orderId,state,createTime,notifyTime,gotTime) VALUES (?,?,?,?,?,?,?,?,?,?)";
        var currTime = new Date().getTime();
        // var defaultTime = this.server.baseData.defaultDateTime;
        object defaultTime = null;
        List<object> values = new List<object> { playerId, id, productId, quantity, fen, orderId, PayLtState.Init, currTime/* index=L-3 */, defaultTime/* index=L-2 */, defaultTime/* index=L-1 */ };
        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[values.Count - 3] = (int)MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 2] = MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 1] = MyDBValueType.DateTime;
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    *insertPayIvyYield(int playerId, int id, string productId, int quantity, string fen, string orderId) {
        var queryStr = "INSERT INTO payivy (playerId,id,productId,quantity,fen,orderId,state,createTime,notifyTime,gotTime) VALUES (?,?,?,?,?,?,?,?,?,?)";
        var currTime = new Date().getTime();
        // var defaultTime = this.server.baseData.defaultDateTime;
        object defaultTime = null;
        List<object> values = new List<object> { playerId, id, productId, quantity, fen, orderId, PayLtState.Init, currTime/* index=L-3 */, defaultTime/* index=L-2 */, defaultTime/* index=L-1 */ };
        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[values.Count - 3] = (int)MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 2] = MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 1] = MyDBValueType.DateTime;
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    // AAA
    *queryPayLt_orderId(string orderId) {
        var queryStr = "SELECT * FROM paylt WHERE orderId=?";
        List<object> values = new List<object> { orderId };
        MsgDBQuery msg = new MsgDBQuery { queryStr = queryStr, values = values };
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        if (r.err != ECode.Success) {
            return r.err;
        }
        var info = this.decodePayLt(r.res);
        return new MyResponse(ECode.Success, info);
    }

    *queryPayLt_p_s_g(int playerId, int state, int got) {
        var queryStr = "SELECT * FROM paylt WHERE playerId=? AND state=? AND got=?";
        List<object> values = new List<object>  { playerId, state, got };
        MsgDBQuery msg = new MsgDBQuery { queryStr = queryStr, values = values };
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        if (r.err != ECode.Success) {
            return r.err;
        }
        SqlTablePayLt infos[] = this.decodePayLtMany(r.res);
        return new MyResponse(ECode.Success, infos);
    }
    *updatePayLtGotMany(string[] orderIds, int got) {
        var queryStr = "UPDATE paylt SET got=?,gotTime=? WHERE orderId IN (";
        for (int i = 0; i < orderIds.Length; i++) {
            queryStr += "?";
            if (i < orderIds.Length - 1) {
                queryStr += ",";
            }
        }
        queryStr += ");";

        List<object> values = new List<object>  { got, new Date().getTime()/* index=1 */ };
        for (int i = 0; i < orderIds.Length; i++) {
            values.Add(orderIds[i]);
        }

        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = orderIds.length,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[1] = (int)MyDBValueType.DateTime;
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    // AAA
    // 根据 orderId 更新 state 和 orderId3, notifyTime
    *updatePayLtStateYield(string orderId, int state, string orderId3) {
        var queryStr = "UPDATE paylt SET state=?,orderId3=?,notifyTime=? WHERE orderId=?";
        List<object> values = new List<object> { state, orderId3, new Date().getTime()/* index=2 */, orderId };
        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[2] = (int)MyDBValueType.DateTime;
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    private SqlTablePayLt decodePayLt(SqlTablePayLt[] sqlDatas) {
        if (sqlDatas == null || sqlDatas.Length != 1) {
            return null;
        }
        var sqlData = sqlDatas[0];
        SqlTablePayLt info = sqlData;
        return info;
    }
    private List<SqlTablePayLt> decodePayLtMany(SqlTablePayLt[] sqlDatas) {
        if (sqlDatas == null) {
            return null;
        }

        List<SqlTablePayLt> array = new List<SqlTablePayLt>();
        for (int i = 0; i < sqlDatas.Length; i++) {
            var sqlData = sqlDatas[i];
            SqlTablePayLt info = sqlData;
            array.Add(info);
        }
        return array;
    }
}