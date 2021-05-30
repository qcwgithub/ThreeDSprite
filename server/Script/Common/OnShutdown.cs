
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnShutdown<T> : Handler<T> where T: Server
    {
        public override MsgType msgType { get { return MsgType.Shutdown; } }

        public override async Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            this.baseScript.setState(ServerState.ShuttingDown);
            await this.baseScript.waitAsync(1000);
            this.baseScript.setState(ServerState.ReadyToShutdown);
            return ECode.Success;
        }
    }
}