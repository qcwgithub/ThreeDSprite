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
            var msg = this.server.CastObject<MsgInsertPlayer>(_msg);
            List<string> fields = new List<string>();
            List<object> values = new List<object>();

            fields.Add("playerId");
            values.Add(msg.player.id);

            #region DBInsertPlayer Auto

            fields.Add("level");
            values.Add(msg.player.level);

            fields.Add("money");
            values.Add(msg.player.money);

            fields.Add("diamond");
            values.Add(msg.player.diamond);

            fields.Add("portrait");
            values.Add(msg.player.portrait);

            fields.Add("userName");
            values.Add(msg.player.userName);

            fields.Add("characterConfigId");
            values.Add(msg.player.characterConfigId);

            #endregion DBInsertPlayer Auto

            List<string> ats = new List<string>();
            for (int i = 0; i < fields.Count; i++)
            {
                ats.Add("@" + i);
            }

            string queryStr = string.Format("INSERT INTO player ({0}) VALUES ({1});", string.Join(",", fields), string.Join(",", ats));
            MySqlParameter[] param = this.MakeParameters(values);

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