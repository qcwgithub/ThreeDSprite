
using System.Collections;
using System.Threading.Tasks;

public class OnShutdown : Handler {
    public override MsgType msgType { get { return MsgType.Shutdown; } }

    public override async Task<MyResponse> handle(ISocket socket, string _msg) {
        this.baseScript.setState(ServerState.ShuttingDown);
        await this.baseScript.waitAsync(1000);
        this.baseScript.setState(ServerState.ReadyToShutdown);
        return ECode.Success;
    }
}