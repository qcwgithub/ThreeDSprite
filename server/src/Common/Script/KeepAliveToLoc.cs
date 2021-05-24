using System.Collections;
using System.Threading.Tasks;

public class KeepAliveToLoc : Handler
{
    public override MsgType msgType { get { return MsgType.KeepAliveToLoc; } }
    public override async Task<MyResponse> handle(object socket, string _msg)
    {
        if (!this.server.network.isConnected(this.baseData.locSocket))
        {
            this.baseData.locNeedReport = true;
            return ECode.Success;
        }

        if (this.baseData.locNeedReport)
        {
            this.baseData.locNeedReport = false;

            var r = await this.baseScript.sendYield(
                this.baseData.locSocket,
                MsgType.LocReportLoc,
                new MsgLocReportLoc { id = this.baseData.id, loc = this.baseScript.myLoc() }
            );
            if (r.err != ECode.Success)
            {
                // console.error("!canStart: " + r.err);
                // process.exit(1);
            }
        }

        return ECode.Success;
    }
}