using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBSavePlayer : DBQueryPlayer
    {
        public override MsgType msgType => MsgType.DBSavePlayer;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgSavePlayer>(_msg);
            List<string> fields = new List<string>();
            List<object> values = new List<object>();

            #region DBSavePlayer Auto

            if (msg.level != null)
            {
                fields.Add("level=@" + values.Count);
                values.Add(msg.level);
            }

            if (msg.money != null)
            {
                fields.Add("money=@" + values.Count);
                values.Add(msg.money);
            }

            if (msg.diamond != null)
            {
                fields.Add("diamond=@" + values.Count);
                values.Add(msg.diamond);
            }

            if (msg.portrait != null)
            {
                fields.Add("portrait=@" + values.Count);
                values.Add(msg.portrait);
            }

            if (msg.userName != null)
            {
                fields.Add("userName=@" + values.Count);
                values.Add(msg.userName);
            }

            if (msg.characterConfigId != null)
            {
                fields.Add("characterConfigId=@" + values.Count);
                values.Add(msg.characterConfigId);
            }


            #endregion DBSavePlayer Auto

            if (fields.Count == 0)
            {
                this.server.logger.ErrorFormat("{0} fields.Count == 0", this.msgName);
                return ECode.Success;
            }

            string queryStr = string.Format("UPDATE player SET {0} WHERE playerId={1};", string.Join(",", fields), msg.playerId);
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