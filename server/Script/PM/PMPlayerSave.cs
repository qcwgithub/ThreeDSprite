using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMPlayerSave : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMPlayerSave; } }
        public override Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            var msg = this.baseScript.decodeMsg<MsgPlayerSCSave>(_msg);
            var player = this.data.GetPlayerInfo(msg.playerId);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} place: {1}, playerId: {2}, player == null!!", this.msgName, msg.place, msg.playerId);
                return Task.FromResult(new MyResponse(ECode.PlayerNotExist));
            }

            var obj = this.server.pmSqlUtils.beginSave(player);
            List<string> buffer = null;
            var last = player.lastProfile;
            var curr = this.server.pmPlayerToSqlTablePlayer.convert(player);

            this.server.pmSqlUtils.endSave(obj);
            player.lastProfile = curr; // 先假设一定成功吧

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(null, buffer.ToArray());
            }
            this.logger.InfoFormat("{0} place: {1}, playerId: {2}, fields: [{3}]", this.msgName, msg.place, player.id, fieldsStr);

            //// reply
            return Task.FromResult(new MyResponse(ECode.Success));
        }
    }
}