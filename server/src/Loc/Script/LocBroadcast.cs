// 这里只处理非游戏需求
public class LocBroadcast : LocHandler {
    public override MsgType msgType { get { return MsgType.Broadcast; } }

    public MyResponse handle(object socket, MsgLocBroadcast msg) {
        this.logger.info("LocBroadcast, ids: %s, msgType: %s", JSON.stringify(msg.ids), msg.msgType.ToString());

        // 只允许全部有效
        for (int i = 0; i < msg.ids.Length; i++) {
            int id = msg.ids[i];
            if (id == this.baseData.id) {
                continue;
            }

            LocServerInfo info;
            if (!this.locData.map.TryGetValue(id, out info) || info == null || info.socket == null || !this.server.netProto.isConnected(info.socket)) {
                this.baseScript.error("LocBroadcast failed, invalid id: " + id);
                r.err = ECode.Error;
                .res = null;
                yield break;
            }
        }

        for (int i = 0; i < msg.ids.length; i++) {
            int id = msg.ids[i];
            if (id == this.baseData.id) {
                this.dispatcher.dispatch(null, msg.msgType, msg.msg, null);
                continue;
            }

            LocServerInfo info = this.locData.map[id];
            this.server.netProto.send(info.socket, msg.msgType, msg.msg, null);
        }
        this.logger.debug("LocBroadcast success");
        r.err = ECode.Success;
        .res = null;
        yield break;
    }
}