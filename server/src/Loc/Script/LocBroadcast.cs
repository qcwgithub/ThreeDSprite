// 这里只处理非游戏需求
using System.Collections;
using System.Threading.Tasks;

public class LocBroadcast : LocHandler
{
    public override MsgType msgType { get { return MsgType.Broadcast; } }

    public override async Task<MyResponse> handle(object socket, string _msg)
    {
        var msg = this.baseScript.castMsg<MsgLocBroadcast>(_msg);
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
                !this.server.network.isConnected(info.socket))
            {
                this.baseScript.error("LocBroadcast failed, invalid id: " + id);
                return ECode.Error;
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
            this.server.network.send(info.socket, msg.msgType, msg.msg, null);
        }
        this.logger.debug("LocBroadcast success");
        return ECode.Success;
    }
}