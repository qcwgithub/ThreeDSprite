using System.Threading.Tasks;
using Data;

namespace Script
{
    public class LocReportLoc : LocHandler
    {
        public override MsgType msgType { get { return MsgType.LocReportLoc; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgLocReportLoc>(_msg);
            // if (msg.id == ServerConst.MONITOR_ID)
            // {
            //     // monitor 不受限制
            //     return ECode.Success.toTask();
            // }
            // this.logger.Info("bbbccc");

            if (msg.serverId == ServerConst.LOC_ID)
            {
                // loc 不允许再次启动
                return ECode.ServerIdUsed.toTask();
            }

            LocServerInfo info;
            if (this.locData.map.TryGetValue(msg.serverId, out info) && this.server.tcpClientScript.isServerConnected(info.serverId))
            {
                this.server.logger.Error("server id used, id: " + msg.serverId);
                return ECode.ServerIdUsed.toTask();
            }

            this.logger.Info("+" + msg.serverId);
            // add to dict
            this.server.data.otherServerSockets[msg.serverId] = socket;

            info = new LocServerInfo();
            info.serverId = msg.serverId;
            info.loc = msg.loc;
            this.locData.map[info.serverId] = info;
            return ECode.Success.toTask();
        }
    }
}