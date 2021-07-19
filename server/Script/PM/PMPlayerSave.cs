using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMPlayerSave : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMPlayerSave; } }
        public override Task<MyResponse> handle(TcpClientData socket/* null */, object _msg)
        {
            var msg = this.server.CastObject<MsgPlayerSCSave>(_msg);
            var player = this.data.GetPlayerInfo(msg.playerId);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} place: {1}, playerId: {2}, player == null!!", this.msgName, msg.place, msg.playerId);
                return ECode.PlayerNotExist.toTask();
            }

            // first reset
            this.server.pmScript.clearDestroyTimer(player);

            MsgSavePlayer msgSave = new MsgSavePlayer();
            msgSave.playerId = player.id;

            List<string> buffer = null;
            SqlTablePlayer last = player.lastProfile;
            SqlTablePlayer curr = this.server.pmPlayerToSqlTable.Convert(player);

            #region PMPlayerSave Auto

            if (last.level != curr.level)
                msgSave.level = curr.level;

            if (last.money != curr.money)
                msgSave.money = curr.money;

            if (last.diamond != curr.diamond)
                msgSave.diamond = curr.diamond;

            if (last.portrait != curr.portrait)
                msgSave.portrait = curr.portrait;

            if (last.userName != curr.userName)
                msgSave.userName = curr.userName;

            if (last.characterConfigId != curr.characterConfigId)
                msgSave.characterConfigId = curr.characterConfigId;


            #endregion PMPlayerSave Auto

            player.lastProfile = curr; // 先假设一定成功吧

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(null, buffer.ToArray());
            }
            this.logger.InfoFormat("{0} place: {1}, playerId: {2}, fields: [{3}]", this.msgName, msg.place, player.id, fieldsStr);

            //// reply
            return ECode.Success.toTask();
        }

        public override MyResponse postHandle(object socket, object _msg, MyResponse r)
        {
            var msg = this.server.CastObject<MsgPlayerSCSave>(_msg);
            var player = this.data.GetPlayerInfo(msg.playerId);
            if (player != null)
            {
                this.server.pmScript.setSaveTimer(player);
            }
            return r;
        }
    }
}