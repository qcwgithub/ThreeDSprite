
using System.Collections;

public class OnShutdown : Handler {
    public override MsgType msgType { get { return MsgType.Shutdown; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse res) {
        this.baseScript.setState(ServerState.ShuttingDown);
        yield return this.baseScript.waitYield(1000);
        this.baseScript.setState(ServerState.ReadyToShutdown);
        res.err = ECode.Success;
        res.res = null;
        yield break;
    }
}