using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBInsertAccount : DBQuery
    {
        public override MsgType msgType => MsgType.DBInsertAccount;

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgDBInsertAccount>(_msg);
            var a = msg.accountInfo;

            var queryStr = "INSERT INTO account (platform, channel, channelUserId, playerId, createTime, oaid, imei, userInfo) VALUES (@0,@1,@2,@3,@4,@5,@6,@7);";
            var param = this.MakeParameters(a.platform, a.channel, a.channelUserId, a.playerId, this.server.SecondsToDateTime(a.createTimeS), a.oaid, a.imei, a.userInfo);

            int affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            return new MyResponse(ECode.Success, null);
        }
    }
}