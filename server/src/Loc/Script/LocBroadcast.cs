// 这里只处理非游戏需求
using System.Collections;
using System.Threading.Tasks;

public class LocBroadcast : LocHandler
{
    public override MsgType msgType { get { return MsgType.Broadcast; } }

    public override Task<MyResponse> handle(ISocket socket, string _msg)
    {
        var msg = this.baseScript.decodeMsg<MsgLocBroadcast>(_msg);
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
                !info.socket.isConnected())
            {
                this.baseScript.error("LocBroadcast failed, invalid id: " + id);
                return Task.FromResult(new MyResponse(ECode.Error, null));
            }
        }

        for (int i = 0; i < msg.ids.Length; i++)
        {
            int id = msg.ids[i];
            if (id == this.baseData.id)
            {
                this.dispatcher.dispatch(null, msg.msgType, this.server.JSON.stringify(msg.msg), null);
                continue;
            }

            LocServerInfo info = this.locData.map[id];
            info.socket.send(msg.msgType, msg.msg, null);
        }
        this.logger.debug("LocBroadcast success");
        return Task.FromResult(new MyResponse(ECode.Success, null));
    }
}