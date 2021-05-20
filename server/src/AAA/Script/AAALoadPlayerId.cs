
using System.Collections;

public class AAALoadPlayerId : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAALoadPlayerId; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        // 这个属于启动时必做的，可以使用 while
        while (true)
        {
            if (!this.server.netProto.isConnected(this.baseData.dbAccountSocket))
            {
                // server.logger.info("AAALoadPlayerId db not connected");
                yield return this.baseScript.waitYield(1000);
            }
            else
            {
                yield return this.server.aaaSqlUtils.selectPlayerIdYield(r);
                if (r.err != ECode.Success)
                {
                    this.baseScript.error("AAALoadPlayerId failed." + r.err);
                }
                else
                {
                    this.aaaData.nextPlayerId = r.res[0].playerId;
                    if (this.baseScript.checkArgs("I", this.aaaData.nextPlayerId))
                    {
                        this.logger.info("AAALoadPlayerId success. nextPlayerId: " + this.aaaData.nextPlayerId);
                        this.aaaData.playerIdReady = true;
                    }
                    break;
                }
            }
        }
        r.err = ECode.Success;
    }
}