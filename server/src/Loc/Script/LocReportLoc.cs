using System.Threading.Tasks;

public class LocReportLoc : LocHandler
{
    public override MsgType msgType { get { return MsgType.LocReportLoc; } }

    public override async Task<MyResponse> handle(object socket, string _msg)
    {
        var msg = this.baseScript.castMsg<MsgLocReportLoc>(_msg);
        this.logger.info("+" + msg.id);
        if (msg.id == ServerConst.MONITOR_ID)
        {
            // monitor 不受限制
            return ECode.Success;
        }

        LocServerInfo info;
        if (this.locData.map.TryGetValue(msg.id, out info) && info != null && info.socket != null && this.server.network.isConnected(info.socket))
        {
            this.baseScript.error("server id used, id: " + msg.id);
            return ECode.ServerIdUsed;
        }

        info = new LocServerInfo();
        info.id = msg.id;
        info.loc = msg.loc;
        info.socket = socket;
        this.locData.map.Add(info.id, info);
        return ECode.Success;
    }
}