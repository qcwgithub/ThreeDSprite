// 这里只处理非游戏需求
using System.Collections;

public class LocBroadcast : LocHandler
{
    public override MsgType msgType { get { return MsgType.Broadcast; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        var msg = _msg as MsgLocBroadcast;
        this.logger.info("LocBroadcast, ids: %s, msgType: %s", this.server.JSON.stringify(msg.ids), msg.msgType.ToString());

        // 只允许全部有效
        for (int i = 0; i < msg.ids.Length; i++)
        {
            int id = msg.ids[i];
            if (id == this.baseData.id)
            {
                continue;
            }

            LocServerInfo info;
            if (!this.locData.map.TryGetValue(id, out info) ||
                info.socket == null ||
                !this.server.netProto.isConnected(info.socket))
            {
                this.baseScript.error("LocBroadcast failed, invalid id: " + id);
                r.err = ECode.Error;
                r.res = null;
                yield break;
            }
        }

        for (int i = 0; i < msg.ids.Length; i++)
        {
            int id = msg.ids[i];
            if (id == this.baseData.id)
            {
                this.dispatcher.dispatch(null, msg.msgType, msg.msg, null);
                continue;
            }

            LocServerInfo info = this.locData.map[id];
            this.server.netProto.send(info.socket, msg.msgType, msg.msg, null);
        }
        this.logger.debug("LocBroadcast success");
        r.err = ECode.Success;
        r.res = null;
        yield break;
    }
}