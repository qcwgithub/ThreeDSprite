using Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Script
{
    public class DBInsertPayiOS : DBQuery
    {
        public override MsgType msgType { get { return MsgType.DBInsertPayiOS; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg/* null */)
        {
            var msg = this.server.CastObject<MsgInsertPayiOS>(_msg);

            string queryStr = "INSERT INTO payios (playerId,env,id,productId,bundleId,quantity,transactionId,originalTransactionId,purchaseDate,expiresDate) VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9)";
            var param = this.makeParameters(msg.playerId, msg.env, msg.id, msg.productId, msg.bundleId, msg.quantity, msg.transactionId, msg.originalTransactionId, msg.purchaseDateMs, msg.expiresDateMs);

            int affectedRows = await MySqlHelper.ExecuteNonQueryAsync(this.dbData.connectionString, queryStr, param);
            return new MyResponse(ECode.Success, null);
        }
    }
}