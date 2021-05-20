
// 监听端口，接收消息体，发送给 AAAPayLtHandleRequest 处理
public class AAAPayLtListenNotify : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAPayLtListenNotify; }

    handle(object socket, object msg) {
        var httpServer = http.createServer((req: http.IncomingMessage, res: http.ServerResponse) => {
            this.handleHttpRequest(req, res);
        });
        httpServer.listen(ServerConst.AAA_LT_NOTIFY_PORT, () => {
            this.server.logger.info("listening lt on " + ServerConst.AAA_LT_NOTIFY_PORT);
        });
        r.err = ECode.Success;
        yield break;
    }

    private handleHttpRequest(req: http.IncomingMessage, res: http.ServerResponse) {
        var body = "";

        req.on("data", _trunk => {
            body += _trunk;
        });

        req.on("end", () => {
            this.logger.debug("body: " + body);
            this.server.baseScript.sendToSelf(MsgType.AAAPayLtHandleNotify, { res: res, body: body });
        });
    }
}