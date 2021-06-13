using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBChangeChannel : DBQuery
    {
        public override MsgType msgType => MsgType.DBChangeChannel;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgDBChangeChannel>(_msg);
            string queryStr = "UPDATE account SET channel=@0, channelUserId=@1, userInfo=@2 WHERE channel=@3 AND channelUserId=@4;";
            MySqlParameter[] param = this.makeParameters(msg.channel2, msg.channelUserId2, msg.userInfo, msg.channel1, msg.channelUserId1);

            int affectedRow = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            return new MyResponse(ECode.Success, null);
        }
    }
}