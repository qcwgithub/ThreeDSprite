
using System.Collections;

public class PMOnDisconnect : OnDisconnect
{
    public override MsgType msgType { get { return MsgType.OnDisconnect; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        base.handle(socket, _msg, r);

        var msg = _msg as MsgOnConnect;
        if (msg.isServer)
        {
            r.err = ECode.Success;
            yield break;
        }

        var player = this.server.netProto.getPlayer(socket);
        if (player == null)
        {
            r.err = ECode.Success;
            yield break;
        }

        if (player.socket != null)
        {
            this.server.netProto.removeCustomMessageListener(player.socket);
            this.server.netProto.unbindPlayerAndSocket(player, player.socket);
        }

        this.server.pmScript.onOffline_calcTimeRelative(player);
        this.server.sqlLog.player_logout(player);

        this.server.pmScript.setDestroyTimer(player, "PMOnDisconnect");

        r.err = ECode.Success;
        yield break;
    }
}