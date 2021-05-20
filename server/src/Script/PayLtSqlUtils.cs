using System.Collections;
using System.Collections.Generic;

public class PayLtSqlUtils : IScript {
    public Server server { get; set; }
    // PM
    public IEnumerator insertPayLtYield(int playerId, int id, string productId, int quantity, string fen, string orderId, MyResponse res) {
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
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, res);
    }

    public IEnumerator insertPayIvyYield(int playerId, int id, string productId, int quantity, string fen, string orderId, MyResponse res) {
        var queryStr = "INSERT INTO payivy (playerId,id,productId,quantity,fen,orderId,state,createTime,notifyTime,gotTime) VALUES (?,?,?,?,?,?,?,?,?,?)";
        var currTime = new Date().getTime();
        // var defaultTime = this.server.baseData.defaultDateTime;
        object defaultTime = null;
        List<object> values = new List<object> { playerId, id, productId, quantity, fen, orderId, PayLtState.Init, currTime/* index=L-3 */, defaultTime/* index=L-2 */, defaultTime/* index=L-1 */ };
        var msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[values.Count - 3] = (int)MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 2] = MyDBValueType.DateTime;
        // msg.valueTypes[values.length - 1] = MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, res);
    }

    // AAA
    public IEnumerator queryPayLt_orderId(string orderId, MyResponse res) {
        var queryStr = "SELECT * FROM paylt WHERE orderId=?";
        List<object> values = new List<object> { orderId };
        MsgDBQuery msg = new MsgDBQuery { queryStr = queryStr, values = values };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, res);
        if (res.err != ECode.Success) {
            yield break;
        }
        res.res = this.decodePayLt(res.res as List<SqlTablePayLt>);
    }

    public IEnumerator queryPayLt_p_s_g(int playerId, int state, int got, MyResponse res) {
        var queryStr = "SELECT * FROM paylt WHERE playerId=? AND state=? AND got=?";
        List<object> values = new List<object>  { playerId, state, got };
        MsgDBQuery msg = new MsgDBQuery { queryStr = queryStr, values = values };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, res);
        if (res.err != ECode.Success) {
            yield break;
        }
        res.res = this.decodePayLtMany(res.res as List<SqlTablePayLt>);
    }
    public IEnumerator updatePayLtGotMany(List<string> orderIds, int got, MyResponse res) {
        var queryStr = "UPDATE paylt SET got=?,gotTime=? WHERE orderId IN (";
        for (int i = 0; i < orderIds.Count; i++) {
            queryStr += "?";
            if (i < orderIds.Count - 1) {
                queryStr += ",";
            }
        }
        queryStr += ");";

        List<object> values = new List<object>  { got, new Date().getTime()/* index=1 */ };
        for (int i = 0; i < orderIds.Count; i++) {
            values.Add(orderIds[i]);
        }

        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = orderIds.Count,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[1] = (int)MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, res);
    }

    // AAA
    // 根据 orderId 更新 state 和 orderId3, notifyTime
    public IEnumerator updatePayLtStateYield(string orderId, int state, string orderId3, MyResponse res) {
        var queryStr = "UPDATE paylt SET state=?,orderId3=?,notifyTime=? WHERE orderId=?";
        List<object> values = new List<object> { state, orderId3, new Date().getTime()/* index=2 */, orderId };
        MsgDBQuery msg = new MsgDBQuery {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[2] = (int)MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, res);
    }

    private SqlTablePayLt decodePayLt(List<SqlTablePayLt> sqlDatas) {
        if (sqlDatas == null || sqlDatas.Count != 1) {
            return null;
        }
        var sqlData = sqlDatas[0];
        SqlTablePayLt info = sqlData;
        return info;
    }
    private List<SqlTablePayLt> decodePayLtMany(List<SqlTablePayLt> sqlDatas) {
        if (sqlDatas == null) {
            return null;
        }

        List<SqlTablePayLt> array = new List<SqlTablePayLt>();
        for (int i = 0; i < sqlDatas.Count; i++) {
            var sqlData = sqlDatas[i];
            SqlTablePayLt info = sqlData;
            array.Add(info);
        }
        return array;
    }
}