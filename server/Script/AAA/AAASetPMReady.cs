
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAASetPMReady : AAAHandler
    {
        public override MsgType msgType { get { return MsgType.AAASetPMReady; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            this.aaaData.pmReady = true;
            this.aaaData.pmReadyTimer = 0;
            return ECode.Success.toTask();
        }
    }
}