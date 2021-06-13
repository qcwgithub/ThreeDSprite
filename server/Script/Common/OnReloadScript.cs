using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnReloadScript : Handler<Server>
    {
        public override MsgType msgType => MsgType.ReloadScript;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            // if (this.server.data.state != ServerState.Started)
            // {
            //     this.server.logger.Info("server state is " + this.server.data.state + ", can not reload script");
            //     return ECode.Error.toTask();
            // }

            bool success = Program.LoadScriptDll();
            this.server.logger.Info("OnReloadScript success ? " + success + " V" + this.server.scriptDllVersion);
            return ECode.Success.toTask();
        }
    }
}