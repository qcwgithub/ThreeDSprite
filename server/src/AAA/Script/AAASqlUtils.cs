
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AAASqlUtils : SqlUtils
{
    public async Task<MyResponse> selectPlayerIdYield()
    {
        var msg = new MsgDBQuery
        {
            queryStr = "SELECT playerId FROM player_id;",
            values = null,
        };
        return await this.server.baseData.dbAccountSocket.sendAsync(MsgType.DBQuery, msg);
    }
    public async Task<MyResponse> updatePlayerIdYield(int playerId)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "UPDATE player_id SET playerId=@0;",
            values = new List<object> { playerId },
            expectedAffectedRows = 1,
        };
        return await this.server.baseData.dbAccountSocket.sendAsync(MsgType.DBQuery, msg);
    }

    private SqlTableAccount decodeAccount(List<SqlTableAccount> sqlDatas)
    {
        if (sqlDatas == null || sqlDatas.Count != 1)
        {
            return null;
        }
        var sqlData = sqlDatas[0];
        SqlTableAccount account = sqlData;
        return account;
    }

    private async Task<MyResponse> doQueryAccountYield(MsgDBQuery msg)
    {
        return await this.server.baseData.dbAccountSocket.sendAsync(MsgType.DBQuery, msg);
    }

    private async Task<MyResponse> queryAccountAndDecode(MsgDBQuery msg)
    {
        var r = await this.server.baseData.dbAccountSocket.sendAsync(MsgType.DBQuery, msg);
        if (r.err != ECode.Success)
        {
            return r;
        }

        r.res = this.decodeAccount(r.res as List<SqlTableAccount>);
        return r;
    }

    public async Task<MyResponse> queryAccountByPlayerIdYield(int playerId)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "SELECT * FROM account WHERE playerId=@0;",
            values = new List<object> { playerId }
        };
        return await this.queryAccountAndDecode(msg);
    }

    // public async Task<MyResponse> queryAccountByChannelIdYield(string channelUserId) {
    //     var msg = new MsgDBQuery {
    //         queryStr: "SELECT * FROM account WHERE channelUserId=?;",
    //         values: [channelUserId]
    //     };
    //     return r = await this.queryAccountAndDecode(msg);
    // }

    public async Task<MyResponse> queryAccountYield(string channel, string channelUserId)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "SELECT * FROM account WHERE channel=@0 AND channelUserId=@1;",
            values = new List<object> { channel, channelUserId }
        };
        return await this.queryAccountAndDecode(msg);
    }
    public Task<MyResponse> queryAccountForChangeChannelYield(string channel, string channelUserId,
        string notExistChannel, string notExistChannelUserId)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "SELECT * FROM account WHERE channel=@0 AND channelUserId=@1 AND NOT EXISTS(SELECT * FROM account WHERE channel=@2 AND channelUserId=@3);",
            values = new List<object> { channel, channelUserId, notExistChannel, notExistChannelUserId }
        };
        return this.queryAccountAndDecode(msg);
    }
    public void unbanAccount(int playerId)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "UPDATE account SET isBan=@0 WHERE playerId=@1;",
            values = new List<object> { false, playerId },
            expectedAffectedRows = 1,
        };
        this.server.baseData.dbAccountSocket.send(MsgType.DBQuery, msg, null);
    }
    public async Task<MyResponse> banAccountYield(int playerId, int unbanTime, string banReason)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "UPDATE account SET isBan=@0,unbanTime=@1,banReason=@2 WHERE playerId=@3;",
            values = new List<object> { true, unbanTime, banReason, playerId },
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[1] = (int)MyDBValueType.DateTime;
        return await this.doQueryAccountYield(msg);
    }

    public async Task<MyResponse> insertAccountYield(SqlTableAccount a)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "INSERT INTO account (platform, channel, channelUserId, playerId, createTime, oaid, imei, userInfo) VALUES (@0,@1,@2,@3,@4,@5,@6,@7);",
            values = new List<object> { a.platform, a.channel, a.channelUserId, a.playerId, a.createTime/* 注意index */, a.oaid, a.imei, a.userInfo },
            expectedAffectedRows = 1,
        };
        msg.valueTypes = new Dictionary<int, int>();
        msg.valueTypes[msg.values.Count - 4] = (int)MyDBValueType.DateTime;
        return await this.server.baseData.dbAccountSocket.sendAsync(MsgType.DBQuery, msg);
    }

    // (channel1, channelUserId1) -> (channel2, channelUserId2)
    public async Task<MyResponse> changeChannelYield(string channel1, string channelUserId1, string channel2, string channelUserId2, string userInfo)
    {
        var msg = new MsgDBQuery
        {
            queryStr = "UPDATE account SET channel=@0, channelUserId=@1, userInfo=@2 WHERE channel=@3 AND channelUserId=@4;",
            values = new List<object> { channel2, channelUserId2, userInfo, channel1, channelUserId1 },
            expectedAffectedRows = 1,
        };
        return await this.doQueryAccountYield(msg);
    }
}