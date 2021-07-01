
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMOnClose : OnSocketClose<PMServer>
    {
        public override MsgType msgType { get { return MsgType.OnSocketClose; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            await base.handle(socket, _msg);

            var msg = (MsgOnClose)_msg;
            // if (msg.isServer)
            // {
            //     return ECode.Success;
            // }

            if (socket.oppositeIsClient)
            {
                var player = (PMPlayerInfo) this.server.tcpClientScript.getPlayer(socket);
                if (player == null)
                {
                    return ECode.Success;
                }

                if (player.socket != null)
                {
                    this.server.tcpClientScript.unbindPlayer(player.socket, player);
                }

                this.server.sqlLog.player_logout(player);

                this.server.pmScript.setDestroyTimer(player, "PMOnClose");
            }

            return ECode.Success;
        }
    }
}