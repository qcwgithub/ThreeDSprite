
public class PMChangeName : PMHandler {
    public override MsgType msgType { get { return MsgType.PMChangeName; } }

    *handle(object socket, MsgChangeName msg) {
        // var data = this.pmData;
        // var script = this.pmScript;
        // var logger = this.logger;

        var player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.PlayerNotExist);
        }
        this.logger.info("%s playerId: %d, %s -> %s", this.msgName, player.id, player.profile.userName, msg.name);

        var res = new ResChangeName();
        var e = this.server.gameScript.changeNameCheck(player, msg, res);
        if (e != ECode.Success) {
            this.pmScript.playerOperError(this, player.id, e);
            return e;
        }

        this.server.gameScript.changeNameExecute(player, msg, res);
        player.profileChanged(PMProfileType.userName);
        this.sqlLog.player_changeName(player);
        return new MyResponse(ECode.Success, res);
    }
}