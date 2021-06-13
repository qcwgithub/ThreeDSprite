
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnShutdown<T> : Handler<T> where T: Server
    {
        public override MsgType msgType { get { return MsgType.Shutdown; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            this.server.setState(ServerState.ShuttingDown);
            await this.server.waitAsync(1000);
            this.server.setState(ServerState.ReadyToShutdown);
            return ECode.Success;
        }
    }
}