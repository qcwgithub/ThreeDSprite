public class LocReportLoc : LocHandler {
    public override MsgType msgType { get { return MsgType.LocReportLoc; } }

    public MyResponse handle(object socket, MsgLocReportLoc msg) {
        this.logger.info("+" + msg.id);
        if (msg.id == ServerConst.MONITOR_ID) {
            // monitor 不受限制
            r.err = ECode.Success;
            .res = null;
            yield break;
        }

        LocServerInfo info;
        if (this.locData.map.TryGetValue(msg.id, out info) && info != null && info.socket != null && this.server.netProto.isConnected(info.socket)) {
            this.baseScript.error("server id used, id: " + msg.id);
            r.err = ECode.ServerIdUsed;
            .res = null;
            yield break;
        }

        info = new LocServerInfo();
        info.id = msg.id;
        info.loc = msg.loc;
        info.socket = socket;
        this.locData.map.Add(info.id, info);
        r.err = ECode.Success;
        .res = null;
        yield break;
    }
}