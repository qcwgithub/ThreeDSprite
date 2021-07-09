
using Data;

namespace Script
{
    public abstract class BMHandler : Handler<BMServer>
    {
        public BMPlayerInfo getPlayer(TcpClientData socket)
        {
            object obj = this.server.tcpClientScript.getPlayer(socket);
            return (obj == null ? null : (BMPlayerInfo)obj);
        }

        public void broadcast(BMBattleInfo battleInfo, MsgType msgType, object msg_, int excludePlayerId = 0)
        {
            foreach (var kv in battleInfo.players)
            {
                BMPlayerInfo playerInfo = kv.Value;
                if (excludePlayerId > 0 && playerInfo.playerId == excludePlayerId)
                {
                    continue;
                }
                if (playerInfo.socket != null && this.server.tcpClientScript.isConnected(playerInfo.socket))
                {
                    this.server.tcpClientScript.send(playerInfo.socket, msgType, msg_, null);
                }
            }
        }
    }
}