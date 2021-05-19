public class LocStart : Handler {
    public override MsgType msgType { get { return MsgType.Start; } }

    public MyResponse handle(object socket, object msg/* no use */) {
        this.baseScript.setState(ServerState.Starting);

        this.baseScript.listen(() => false);

        this.baseScript.setState(ServerState.Started);
        return MyResponse.create(ECode.Success);
    }
}