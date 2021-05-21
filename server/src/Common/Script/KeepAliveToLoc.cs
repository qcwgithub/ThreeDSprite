using System.Collections;

public class KeepAliveToLoc : Handler
{
    public override MsgType msgType { get { return MsgType.KeepAliveToLoc; } }
    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        if (!this.server.netProto.isConnected(this.baseData.locSocket))
        {
            this.baseData.locNeedReport = true;
            r.err = ECode.Success;
            yield break;
        }

        if (this.baseData.locNeedReport)
        {
            this.baseData.locNeedReport = false;

            yield return this.baseScript.sendYield(
                this.baseData.locSocket,
                MsgType.LocReportLoc,
                new MsgLocReportLoc { id = this.baseData.id, loc = this.baseScript.myLoc() },
                r
            );
            if (r.err != ECode.Success)
            {
                // console.error("!canStart: " + r.err);
                // process.exit(1);
            }
        }

        r.err = ECode.Success;
        yield break;
    }
}