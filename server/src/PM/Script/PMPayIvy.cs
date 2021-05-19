
// 客户端完成支付后，会发送此消息来领奖励
public class PMPayIvy : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPayIvy; } }

    *handle(object socket, MsgPay msg) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.PlayerNotExist);
        }
        this.logger.info("%s playerId: %d", this.msgName, player.id);

        var vipScript = this.server.vipScript;
        var logger = this.logger;
        if (player.ivyPaying) {
            logger.warn("playerId %d ivyPaying", player.id);
            return MyResponse.create(ECode.LastPayNotEnd);
        }

        var res = new ResPay();
        var e = vipScript.payCheck(player, msg, res);
        if (e != ECode.Success) {
            return e;
        }

        player.ivyPaying = true;

        MyResponse r = yield this.server.payIvySqlUtils.queryPayIvy_p_s_g(player.id, PayIvyState.Succeeded, 0);
        if (r.err != ECode.Success) {
            return r.err;
        }

        SqlTablePayIvy infos[] = r.res;
        if (infos == null || infos.length == 0) {
            return new MyResponse(ECode.Success, res);
        }

        var string orderIds[] = [];
        // var iapConfig = this.server.pmData.iapConfig;
        for (int i = 0; i < infos.length; i++) {
            SqlTablePayIvy info = infos[i];
            var e2 = this.server.vipScript.payCheckIvy(player, msg, res, info.id, info.productId, info.orderId);
            if (e2 == ECode.Success) {
                orderIds.push(info.orderId);
            }
        }

        r = yield this.server.payIvySqlUtils.updatePayIvyGotMany(orderIds, 1);
        if (r.err != ECode.Success) {
            return r.err;
        }

        //// ok
        vipScript.payExecute(player, msg, res);

        this.pmScript.postHandlePayResult(player, res);
        return new MyResponse(ECode.Success, res);
    }

    // 
    postHandle(object socket, MsgPay msg) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player != null) {
            this.logger.info("%s postHandle playerId: %d", this.msgName, player.id);
            player.ivyPaying = false;
        }
        else {
            this.logger.info("%s postHandle playerId: null", this.msgName);
        }
    }
}