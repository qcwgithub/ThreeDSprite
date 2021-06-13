
using System.Collections.Generic;
using Data;

namespace Script
{
    public class SqlLog : IServerScript<Server>
    {
        public Server server { get; set; }

        private void doQuery(MsgType msgType, object msg)
        {
            this.server.tcpClientScript.sendToServer(ServerConst.DB_LOG_ID, msgType, msg, (e, r) =>
            {
                if (e != ECode.Success)
                {
                    this.server.logger.ErrorFormat("SqlLog.doQuery failed. {0}", msgType);
                }
            });
        }

        public void player_login(PMPlayerInfo player)
        {
            var msg = new MsgLogPlayerLogin { playerId = player.id };
            this.doQuery(MsgType.DBLogPlayerLogin, msg);
        }

        public void player_logout(PMPlayerInfo player)
        {
            var msg = new MsgLogPlayerLogout { playerId = player.id };
            this.doQuery(MsgType.DBLogPlayerLogout, msg);
        }

        public void account_changeChannel(int playerId, string channel1, string channelUserId1, string channel2, string channelUserId2)
        {
            var msg = new MsgLogChangeChannel
            {
                playerId = playerId,
                channel1 = channel1,
                channelUserId1 = channelUserId1,
                channel2 = channel2,
                channelUserId2 = channelUserId2
            };
            this.doQuery(MsgType.DBChangeChannel, msg);
        }
    }
}