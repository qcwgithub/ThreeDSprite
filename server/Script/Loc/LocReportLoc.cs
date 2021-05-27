using System.Threading.Tasks;
using Data;

public class LocReportLoc : LocHandler
{
    public override MsgType msgType { get { return MsgType.LocReportLoc; } }

    public override Task<MyResponse> handle(ISocket socket, string _msg)
    {
        var msg = this.baseScript.decodeMsg<MsgLocReportLoc>(_msg);
        this.logger.Info("+" + msg.id);
        if (msg.id == ServerConst.MONITOR_ID)
        {
            // monitor 不受限制
            return Task.FromResult(new MyResponse(ECode.Success));
        }

        LocServerInfo info;
        if (this.locData.map.TryGetValue(msg.id, out info) && info != null && info.socket != null && info.socket.isConnected())
        {
            this.server.logger.Error("server id used, id: " + msg.id);
            return Task.FromResult(new MyResponse(ECode.ServerIdUsed));
        }

        info = new LocServerInfo();
        info.id = msg.id;
        info.loc = msg.loc;
        info.socket = socket;
        this.locData.map.Add(info.id, info);
        return Task.FromResult(new MyResponse(ECode.Success));
    }
}