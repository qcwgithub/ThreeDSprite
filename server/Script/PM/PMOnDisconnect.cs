
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMOnDisconnect : OnDisconnect<PMServer>
    {
        public override MsgType msgType { get { return MsgType.OnDisconnect; } }

        public override async Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            await base.handle(socket, _msg);

            var msg = this.baseScript.decodeMsg<MsgOnConnect>(_msg);
            if (msg.isServer)
            {
                return ECode.Success;
            }

            var player = this.tcpClientScript.getPlayer(socket);
            if (player == null)
            {
                return ECode.Success;
            }

            if (player.socket != null)
            {
                this.tcpClientScript.unbindPlayer(player.socket, player);
            }

            this.server.sqlLog.player_logout(player);

            this.server.pmScript.setDestroyTimer(player, "PMOnDisconnect");

            return ECode.Success;
        }
    }
}