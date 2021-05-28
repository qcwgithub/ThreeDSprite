
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Script
{
    public class AAALoadPlayerId : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAALoadPlayerId; } }

        public override async Task<MyResponse> handle(ISocket socket, string _msg)
        {
            // 这个属于启动时必做的，可以使用 while
            while (true)
            {
                if (!this.baseData.dbAccountSocket.isConnected())
                {
                    // server.logger.info("AAALoadPlayerId db not connected");
                    await this.baseScript.waitAsync(1000);
                }
                else
                {
                    var r = await this.server.aaaSqlUtils.selectPlayerIdYield();
                    if (r.err != ECode.Success)
                    {
                        this.logger.Error("AAALoadPlayerId failed." + r.err);
                    }
                    else
                    {
                        var dict = this.server.JSON.parse<Dictionary<string, List<object>>>(r.res as string);
                        this.aaaData.nextPlayerId = Convert.ToInt32(dict["playerId"][0]);
                        if (this.aaaData.nextPlayerId > 0)
                        {
                            this.logger.Info("AAALoadPlayerId success. nextPlayerId: " + this.aaaData.nextPlayerId);
                            this.aaaData.playerIdReady = true;
                        }
                        break;
                    }
                }
            }
            return ECode.Success;
        }
    }
}