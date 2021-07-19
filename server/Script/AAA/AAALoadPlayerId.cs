
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAALoadPlayerId : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAALoadPlayerId; } }

        public override async Task<MyResponse> handle(TcpClientData socket/* null */, object _msg/* null */)
        {
            this.logger.Info(this.msgName + " V" + this.server.scriptDllVersion);
            if (!this.server.tcpClientScript.isServerConnected(ServerConst.DB_ACCOUNT_ID))
            {
                // server.logger.info("AAALoadPlayerId db not connected");
                return ECode.Success;
            }

            if (this.aaaData.playerIdReady)
            {
                return ECode.Success;
            }

            var r = await this.server.aaaSqlUtils.selectPlayerIdAsync();
            if (r.err != ECode.Success)
            {
                this.logger.Error("AAALoadPlayerId failed." + r.err);
                return ECode.Error;
            }
            else
            {
                this.aaaData.nextPlayerId = this.server.CastObject<ResDBQueryAccountPlayerId>(r.res).playerId;
                if (this.aaaData.nextPlayerId > 0)
                {
                    this.logger.Info("AAALoadPlayerId success. nextPlayerId: " + this.aaaData.nextPlayerId);
                    this.aaaData.playerIdReady = true;
                    return ECode.Success;
                }
                else
                {
                    return ECode.Error;
                }
            }

        }

        public override MyResponse postHandle(object socket, object msg, MyResponse r)
        {
            if (this.aaaData.nextPlayerId == 0)
            {
                this.server.setTimer(1, this.msgType, null);
            }
            return r;
        }
    }
}