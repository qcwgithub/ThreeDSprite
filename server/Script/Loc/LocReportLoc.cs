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

            if (msg.id == ServerConst.LOC_ID)
            {
                // loc 不允许再次启动
                return ECode.ServerIdUsed.toTask();
            }

            LocServerInfo info;
            if (this.locData.map.TryGetValue(msg.id, out info) && this.server.tcpClientScript.isServerConnected(info.id))
            {
                this.server.logger.Error("server id used, id: " + msg.id);
                return ECode.ServerIdUsed.toTask();
            }

            this.logger.Info("+" + msg.id);
            // add to dict
            this.server.data.otherServerSockets[msg.id] = socket;

            info = new LocServerInfo();
            info.id = msg.id;
            info.loc = msg.loc;
            this.locData.map[info.id] = info;
            return ECode.Success.toTask();
        }
    }
}