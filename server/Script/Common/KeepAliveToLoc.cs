using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class KeepAliveToLoc<T> : Handler<T> where T: Server
    {
        public override MsgType msgType { get { return MsgType.KeepAliveToLoc; } }
        public override async Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            if (!this.server.tcpClientScript.isConnected(this.baseData.locSocket))
            {
                this.baseData.locNeedReport = true;
                return ECode.Success;
            }


            if (this.baseData.locNeedReport)
            {
                int id = this.server.baseScript.myLoc().id;
                this.server.logger.Info("Keey alive to loc " + id);
                this.baseData.locNeedReport = false;

                var r = await this.tcpClientScript.sendAsync(this.baseData.locSocket, 
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
}