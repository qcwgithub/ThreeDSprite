
public class OnShutdown : Handler {
    public override MsgType msgType { get { return MsgType.Shutdown; } }

    public override MyResponse handle(object socket, object msg/* no use */) {
        this.baseScript.setState(ServerState.ShuttingDown);
        yield this.baseScript.waitYield(1000);
        this.baseScript.setState(ServerState.ReadyToShutdown);
        return MyResponse.create(ECode.Success, null)
    }
}