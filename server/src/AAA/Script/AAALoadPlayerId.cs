
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AAALoadPlayerId : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAALoadPlayerId; } }

    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        // 这个属于启动时必做的，可以使用 while
        while (true)
        {
            if (!this.server.netProto.isConnected(this.baseData.dbAccountSocket))
            {
                // server.logger.info("AAALoadPlayerId db not connected");
                await this.baseScript.waitYield(1000);
            }
            else
            {
                var r = await this.server.aaaSqlUtils.selectPlayerIdYield();
                if (r.err != ECode.Success)
                {
                    this.baseScript.error("AAALoadPlayerId failed." + r.err);
                }
                else
                {
                    this.aaaData.nextPlayerId = (int)(r.res as Dictionary<string, object>)["playerId"];
                    if (this.aaaData.nextPlayerId > 0)
                    {
                        this.logger.info("AAALoadPlayerId success. nextPlayerId: " + this.aaaData.nextPlayerId);
                        this.aaaData.playerIdReady = true;
                    }
                    break;
                }
            }
        }
        return ECode.Success;
    }
}