using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBInsertPlayer : DBQueryPlayer
    {
        public override MsgType msgType => MsgType.DBInsertPlayer;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgInsertPlayer>(_msg);
            string queryStr = "INSERT INTO player (id) VALUES (@0);";
            MySqlParameter[] param = this.makeParameters(msg.player.id);

            int affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            if (affectedRows == 1)
            {
                return ECode.Success;
            }
            else
            {
                return ECode.Error;
            }
        }
    }
}