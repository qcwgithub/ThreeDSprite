using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBLogChangeChannel : DBQuery
    {
        public override MsgType msgType => MsgType.DBLogChangeChannel;

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgLogChangeChannel>(_msg);

            var queryStr = "INSERT INTO account_changeChannel(playerId,channel1,channelUserId1,channel2,channelUserId2) VALUES (@0,@1,@2,@3,@4);";
            var param = this.makeParameters(msg.playerId, msg.channel1, msg.channelUserId1, msg.channel2, msg.channelUserId2);
            int affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            return new MyResponse(ECode.Success, null);
        }
    }
}