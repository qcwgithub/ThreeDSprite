
using System.Collections;
using System.Threading.Tasks;

public class PMOnDisconnect : OnDisconnect
{
    public override MsgType msgType { get { return MsgType.OnDisconnect; } }

    public override async Task<MyResponse> handle(object socket, string _msg)
    {
        await base.handle(socket, _msg);

        var msg = this.baseScript.castMsg<MsgOnConnect>(_msg);
        if (msg.isServer)
        {
            return ECode.Success;
        }

        var player = this.server.network.getPlayer(socket);
        if (player == null)
        {
            return ECode.Success;
        }

        if (player.socket != null)
        {
            this.server.network.removeCustomMessageListener(player.socket);
            this.server.network.unbindPlayerAndSocket(player, player.socket);
        }
        
        this.server.sqlLog.player_logout(player);

        this.server.pmScript.setDestroyTimer(player, "PMOnDisconnect");

        return ECode.Success;
    }
}