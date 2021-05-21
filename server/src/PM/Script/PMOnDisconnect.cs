
using System.Collections;
using System.Threading.Tasks;

public class PMOnDisconnect : OnDisconnect
{
    public override MsgType msgType { get { return MsgType.OnDisconnect; } }

    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        await base.handle(socket, _msg);

        var msg = _msg as MsgOnConnect;
        if (msg.isServer)
        {
            return ECode.Success;
        }

        var player = this.server.netProto.getPlayer(socket);
        if (player == null)
        {
            return ECode.Success;
        }

        if (player.socket != null)
        {
            this.server.netProto.removeCustomMessageListener(player.socket);
            this.server.netProto.unbindPlayerAndSocket(player, player.socket);
        }
        
        this.server.sqlLog.player_logout(player);

        this.server.pmScript.setDestroyTimer(player, "PMOnDisconnect");

        return ECode.Success;
    }
}