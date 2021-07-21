// 这里只处理非游戏需求
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class LocBroadcast : LocHandler
    {
        public override MsgType msgType { get { return MsgType.Broadcast; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgLocBroadcast>(_msg);
            this.logger.InfoFormat("LocBroadcast, ids: {0}, msgType: {1}", JsonUtils.stringify(msg.serverIds), msg.msgType.ToString());

            // 只允许全部有效
            for (int i = 0; i < msg.serverIds.Count; i++)
            {
                int serverIdd = msg.serverIds[i];
                if (serverIdd == this.baseData.serverId)
                {
                    continue;
                }

                LocServerInfo info;
                if (!this.locData.map.TryGetValue(serverIdd, out info) ||
                    !this.server.tcpClientScript.isServerConnected(info.serverId))
                {
                    this.logger.Error("LocBroadcast failed, invalid id: " + serverIdd);
                    return ECode.Error.toTask();
                }
            }

            for (int i = 0; i < msg.serverIds.Count; i++)
            {
                int serverId = msg.serverIds[i];
                if (serverId == this.baseData.serverId)
                {
                    this.server.proxyDispatch(socket, msg.msgType, msg.getMsg(), null);
                    continue;
                }

                LocServerInfo info = this.locData.map[serverId];
                this.server.tcpClientScript.sendToServer(info.serverId, msg.msgType, msg.getMsg(), null);
            }
            this.logger.Debug("LocBroadcast success");
            return ECode.Success.toTask();
        }
    }
}