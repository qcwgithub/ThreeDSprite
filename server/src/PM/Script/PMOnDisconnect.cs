
public class PMOnDisconnect : OnDisconnect {
    public override MsgType msgType { get { return MsgType.OnDisconnect; } }

    handle(object socket, msg: { object socket, isListen: boolean, isServer: boolean }) {
        super.handle(socket, msg);

        if (msg.isServer) {
            return MyResponse.create(ECode.Success);
        }

        var player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.Success);
        }

        if (player.socket != null) {
            this.server.netProto.removeCustomMessageListener(player.socket);
            this.server.netProto.unbindPlayerAndSocket(player, player.socket);
        }

        this.server.pmScript.onOffline_calcTimeRelative(player);
        this.server.sqlLog.player_logout(player);

        this.server.pmScript.setDestroyTimer(player, "PMOnDisconnect");

        return MyResponse.create(ECode.Success);
    }
}