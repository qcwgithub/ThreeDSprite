
public class AAASqlUtils : SqlUtils {
    *selectPlayerIdYield() {
        MsgDBQuery msg = {
            queryStr: "SELECT playerId FROM player_id;",
            values: []
        };
        return yield this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg);
    }
    *updatePlayerIdYield(int playerId) {
        MsgDBQuery msg = {
            queryStr: "UPDATE player_id SET playerId=?;",
            values: [playerId],
            expectedAffectedRows: 1,
        };
        return yield this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg);
    }

    private decodeAccount(SqlTableAccount[] sqlDatas): SqlTableAccount {
        if (sqlDatas == null || sqlDatas.length != 1) {
            return null;
        }
        var sqlData = sqlDatas[0];
        SqlTableAccount account = sqlData;
        return account;
    }

    private *doQueryAccountYield(msg: MsgDBQuery) {
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg);
        return r;
    }

    private *queryAccountAndDecode(msg: MsgDBQuery) {
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg);
        if (r.err != ECode.Success) {
            return r;
        }

        return new MyResponse(ECode.Success, this.decodeAccount(r.res));
    }

    *queryAccountByPlayerIdYield(int playerId) {
        MsgDBQuery msg = {
            queryStr: "SELECT * FROM account WHERE playerId=?;",
            values: [playerId]
        };
        return yield this.queryAccountAndDecode(msg);
    }

    // *queryAccountByChannelIdYield(string channelUserId) {
    //     MsgDBQuery msg = {
    //         queryStr: "SELECT * FROM account WHERE channelUserId=?;",
    //         values: [channelUserId]
    //     };
    //     return yield this.queryAccountAndDecode(msg);
    // }

    *queryAccountYield(string channel, string channelUserId) {
        MsgDBQuery msg = {
            queryStr: "SELECT * FROM account WHERE channel=? AND channelUserId=?;",
            values: [channel, channelUserId]
        };
        return yield this.queryAccountAndDecode(msg);
    }
    *queryAccountForChangeChannelYield(string channel, string channelUserId,
        string notExistChannel, string notExistChannelUserId) {
        MsgDBQuery msg = {
            queryStr: "SELECT * FROM account WHERE channel=? AND channelUserId=? AND NOT EXISTS(SELECT * FROM account WHERE channel=? AND channelUserId=?);",
            values: [channel, channelUserId, notExistChannel, notExistChannelUserId]
        };
        return yield this.queryAccountAndDecode(msg);
    }
    unbanAccount(int playerId) {
        MsgDBQuery msg = {
            queryStr: "UPDATE account SET isBan=? WHERE playerId=?;",
            values: [false, playerId],
            expectedAffectedRows: 1,
        };
        this.server.baseScript.send(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg);
    }
    *banAccountYield(int playerId, int unbanTime, string banReason) {
        MsgDBQuery msg = {
            queryStr: "UPDATE account SET isBan=?,unbanTime=?,banReason=? WHERE playerId=?;",
            values: [true, unbanTime, banReason, playerId],
            expectedAffectedRows: 1,
        };
        msg.valueTypes = {};
        msg.valueTypes[1] = MyDBValueType.DateTime;
        return yield this.doQueryAccountYield(msg);
    }

    *insertAccountYield(a: SqlTableAccount) {
        MsgDBQuery msg = {
            queryStr: "INSERT INTO account (platform, channel, channelUserId, playerId, createTime, oaid, imei, userInfo) VALUES (?,?,?,?,?,?,?,?);",
            values: [a.platform, a.channel, a.channelUserId, a.playerId, a.createTime/* 注意index */, a.oaid, a.imei, a.userInfo],
            expectedAffectedRows: 1,
        };
        msg.valueTypes = {};
        msg.valueTypes[msg.values.length - 4] = MyDBValueType.DateTime;
        return yield this.server.baseScript.sendYield(this.server.baseData.dbAccountSocket, MsgType.DBQuery, msg);
    }

    // (channel1, channelUserId1) -> (channel2, channelUserId2)
    *changeChannelYield(string channel1, string channelUserId1, string channel2, string channelUserId2, string userInfo) {
        MsgDBQuery msg = {
            queryStr: "UPDATE account SET channel=?, channelUserId=?, userInfo=? WHERE channel=? AND channelUserId=?;",
            values: [channel2, channelUserId2, userInfo, channel1, channelUserId1],
            expectedAffectedRows: 1,
        };
        return yield this.doQueryAccountYield(msg);
    }


}