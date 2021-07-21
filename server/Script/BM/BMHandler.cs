
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
                BMPlayer player = kv.Value;
                if (excludePlayerId > 0 && player.playerId == excludePlayerId)
                {
                    continue;
                }
                if (player.socket != null && player.socket.isConnected())
                {
                    player.socket.send(msgType, msg_, null);
                }
            }
        }
    }
}