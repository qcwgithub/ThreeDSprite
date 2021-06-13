using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class KeepAliveToLoc<T> : Handler<T> where T: Server
    {
        public override MsgType msgType { get { return MsgType.KeepAliveToLoc; } }
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            if (!this.server.tcpClientScript.isServerConnected(ServerConst.LOC_ID))
            {
                this.baseData.needReportToLoc = true;
                return ECode.Success;
            }

            if (this.baseData.needReportToLoc)
            {
                int id = this.server.myLoc().id;
                this.server.logger.Info("Keep alive to loc " + id);
                this.baseData.needReportToLoc = false;

                var r = await this.server.tcpClientScript.sendToServerAsync(ServerConst.LOC_ID, 
                    MsgType.LocReportLoc,
                    new MsgLocReportLoc { id = this.baseData.id, loc = this.server.myLoc() }
                );
                if (r.err != ECode.Success)
                {
                    // console.error("!canStart: " + r.err);
                    // process.exit(1);
                    this.server.logger.ErrorFormat("Keep alive to loc, error: {0}, exit now.", r.err);
                    // Environment.Exit(0);
                }
            }

            return ECode.Success;
        }

        public override MyResponse postHandle(object socket, object msg, MyResponse r)
        {
            this.server.setTimer(3, this.msgType, null);
            return r;
        }
    }
}