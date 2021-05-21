
using System.Collections;
using System.Collections.Generic;

public class AAASqlUtils : SqlUtils {
    public IEnumerator selectPlayerIdYield(MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "SELECT playerId FROM player_id;",
            values = null,
        };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg, res);
    }
    public IEnumerator updatePlayerIdYield(int playerId, MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "UPDATE player_id SET playerId=@0;",
            values = new List<object> { playerId },
            expectedAffectedRows = 1,
        };
        yield return this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg, res);
    }

    private SqlTableAccount decodeAccount(List<SqlTableAccount> sqlDatas) {
        if (sqlDatas == null || sqlDatas.Count != 1) {
            return null;
        }
        var sqlData = sqlDatas[0];
        SqlTableAccount account = sqlData;
        return account;
    }

    private IEnumerator doQueryAccountYield(MsgDBQuery msg, MyResponse res) {
        yield return this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg, res);
    }

    private IEnumerator queryAccountAndDecode(MsgDBQuery msg, MyResponse res) {
        yield return this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg, res);
        if (res.err != ECode.Success) {
            yield break;
        }

        res.res = this.decodeAccount(res.res as List<SqlTableAccount>);
    }

    public IEnumerator queryAccountByPlayerIdYield(int playerId, MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "SELECT * FROM account WHERE playerId=@0;",
            values = new List<object>  { playerId }
        };
        return this.queryAccountAndDecode(msg, res);
    }

    // public IEnumerator queryAccountByChannelIdYield(string channelUserId, MyResponse r) {
    //     var msg = new MsgDBQuery {
    //         queryStr: "SELECT * FROM account WHERE channelUserId=?;",
    //         values: [channelUserId]
    //     };
    //     return yield return this.queryAccountAndDecode(msg);
    // }

    public IEnumerator queryAccountYield(string channel, string channelUserId, MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "SELECT * FROM account WHERE channel=@0 AND channelUserId=@1;",
            values = new List<object> { channel, channelUserId }
        };
        return this.queryAccountAndDecode(msg, res);
    }
    public IEnumerator queryAccountForChangeChannelYield(string channel, string channelUserId,
        string notExistChannel, string notExistChannelUserId, MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "SELECT * FROM account WHERE channel=@0 AND channelUserId=@1 AND NOT EXISTS(SELECT * FROM account WHERE channel=@2 AND channelUserId=@3);",
            values = new List<object> { channel, channelUserId, notExistChannel, notExistChannelUserId }
        };
        return this.queryAccountAndDecode(msg, res);
    }
    public void unbanAccount(int playerId) {
        var msg = new MsgDBQuery {
            queryStr = "UPDATE account SET isBan=@0 WHERE playerId=@1;",
            values = new List<object> { false, playerId },
            expectedAffectedRows = 1,
        };
        this.server.baseScript.send(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg);
    }
    public IEnumerator banAccountYield(int playerId, int unbanTime, string banReason, MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "UPDATE account SET isBan=@0,unbanTime=@1,banReason=@2 WHERE playerId=@3;",
            values = new List<object> { true, unbanTime, banReason, playerId },
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[1] = (int)MyDBValueType.DateTime;
        return this.doQueryAccountYield(msg, res);
    }

    public IEnumerator insertAccountYield(SqlTableAccount a, MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "INSERT INTO account (platform, channel, channelUserId, playerId, createTime, oaid, imei, userInfo) VALUES (@0,@1,@2,@3,@4,@5,@6,@7);",
            values = new List<object> { a.platform, a.channel, a.channelUserId, a.playerId, a.createTime/* 注意index */, a.oaid, a.imei, a.userInfo },
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[msg.values.Count - 4] = (int)MyDBValueType.DateTime;
        yield return this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg, res);
    }

    // (channel1, channelUserId1) -> (channel2, channelUserId2)
    public IEnumerator changeChannelYield(string channel1, string channelUserId1, string channel2, string channelUserId2, string userInfo, MyResponse res) {
        var msg = new MsgDBQuery {
            queryStr = "UPDATE account SET channel=@0, channelUserId=@1, userInfo=@2 WHERE channel=@3 AND channelUserId=@4;",
            values = new List<object> { channel2, channelUserId2, userInfo, channel1, channelUserId1 },
            expectedAffectedRows = 1,
        };
        return this.doQueryAccountYield(msg, res);
    }
}