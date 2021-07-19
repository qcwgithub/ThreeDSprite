
using Data;

namespace Script
{
    public abstract class BMHandler : Handler<BMServer>
    {
        public BMPlayer getPlayer(TcpClientData socket)
        {
            object obj = this.server.tcpClientScript.getPlayer(socket);
            return (obj == null ? null : (BMPlayer)obj);
        }

        public void broadcast(BMBattle battle, MsgType msgType, object msg_, int excludePlayerId = 0)
        {
            foreach (var kv in battle.playerDict)
            {
                BMPlayer playerInfo = kv.Value;
                if (excludePlayerId > 0 && playerInfo.playerId == excludePlayerId)
                {
                    continue;
                }
                if (playerInfo.socket != null && playerInfo.socket.isConnected())
                {
                    playerInfo.socket.send(msgType, msg_, null);
                }
            }
        }
    }
}