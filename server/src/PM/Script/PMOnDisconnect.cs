
using System.Collections;
using System.Threading.Tasks;

public class PMOnDisconnect : OnDisconnect
{
    public override MsgType msgType { get { return MsgType.OnDisconnect; } }

    public override async Task<MyResponse> handle(ISocket socket, string _msg)
    {
        await base.handle(socket, _msg);

        var msg = this.baseScript.decodeMsg<MsgOnConnect>(_msg);
        if (msg.isServer)
        {
            return ECode.Success;
        }

        var player = socket.getPlayer();
        if (player == null)
        {
            return ECode.Success;
        }

        if (player.socket != null)
        {
            player.socket.removeCustomMessageListener();
            player.socket.unbindPlayer(player);
        }
        
        this.server.sqlLog.player_logout(player);

        this.server.pmScript.setDestroyTimer(player, "PMOnDisconnect");

        return ECode.Success;
    }
}