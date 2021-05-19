
public class AAAPayLtHandleNotify : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAPayLtHandleNotify; }

    // https://wiki.g-bits.com/pages/viewpage.action?pageId=559682334
    // 检查签名
    private checkSign(body: LtPayResult) {
        var string buffer[] = [];
        buffer.push("amount=" + body.amount);
        buffer.push("channelNo=" + body.channelNo);
        buffer.push("gameOrderNo=" + body.gameOrderNo);
        if (body.productId != undefined) {
            buffer.push("productId=" + body.productId);
        }
        buffer.push("status=" + body.status);
        buffer.push("thirdNo=" + body.thirdNo);

        buffer.push("key=540b91bffa2eeaab06dffe25b2642562");

        var signInput = buffer.join("&");
        var sign = md5(signInput);
        if (sign != body.sign) {
            this.baseScript.error("sign error, input(%s) sign(%s) lt.sign(%s)", signInput, sign, body.sign);
            return MyResponse.create(ECode.InvalidSign);
        }
        return MyResponse.create(ECode.Success);
    }

    *handle(object socket, msg: { res: http.ServerResponse, string body }) {
        var res = msg.res;
        this.logger.info("AAAPayLtHandleRequest body: %s", msg.body);
        var body = querystring.parse(msg.body) as any as LtPayResult;

        // 检验参数
        if (!this.server.scUtils.checkArgs("SSSSS", body.status, body.amount, body.channelNo, body.gameOrderNo, body.sign)) {
            this.respond(res, 2, "1 of these is invalid: status,amount,channelNo,gameOrderNo,sign");
            return MyResponse.create(ECode.InvalidParam);
        }

        // 检查签名
        var err = this.checkSign(body);
        if (err != ECode.Success) {
            this.respond(res, 2, "sign error");
            return err;
        }

        // 根据游戏订单号查询
        MyResponse r = yield this.server.payLtSqlUtils.queryPayLt_orderId(body.gameOrderNo);
        if (r.err != ECode.Success) {
            this.baseScript.error("query gameOrderNo(%s) failed", body.gameOrderNo);
            this.respond(res, 2, "query gameOrderNo failed, " + r.err);
            return r.err;
        }

        SqlTablePayLt info = r.res;
        if (info == null) {
            this.baseScript.error("query gameOrderNo(%s) info==null", body.gameOrderNo);
            this.respond(res, 2, "query gameOrderNo failed, info==null");
            return MyResponse.create(ECode.OrderIdNotExist);
        }

        if (body.amount != info.fen) {
            this.baseScript.error("body.amount(%s) != info.fen(%s)", body.amount, info.fen);
            this.respond(res, 2, "money not equal");
            return MyResponse.create(ECode.InvalidParam);
        }

        if (info.state == PayLtState.Succeeded) {
            this.respond(res, 0, "succeeded before");
            return MyResponse.create(ECode.Success);
        }

        if (info.state == PayLtState.Failed) {
            this.respond(res, 0, "failed before");
            return MyResponse.create(ECode.Success);
        }

        // 更改状态为成功，或失败
        var newState = (body.status == "success" ? PayLtState.Succeeded : PayLtState.Failed);
        r = yield this.server.payLtSqlUtils.updatePayLtStateYield(body.gameOrderNo, newState, body.thirdNo);
        if (r.err != ECode.Success) {
            this.respond(res, 2, "update state failed");
            return r.err;
        }

        this.respond(res, 0, "ok");
        return MyResponse.create(ECode.Success);
    }

    private respond(res: http.ServerResponse, int status, string message) {
        res.writeHead(200, { "Content-Type": "text/json" });
        var json = { status: status, message: message };
        res.write(JSON.stringify(json));
        res.end();
    }
}