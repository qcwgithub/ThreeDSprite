using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnStart : Handler<Server>
    {
        public override MsgType msgType { get { return MsgType.Start; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var data = this.server.data;

            if (data.state != ServerState.Initing)
            {
                return ECode.Success.toTask();
            }

            this.server.setState(ServerState.Starting);
            this.server.OnStart();
            this.server.setState(ServerState.Started);
            return ECode.Success.toTask();
        }
    }
}