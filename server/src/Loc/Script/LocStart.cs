using System.Threading.Tasks;

public class LocStart : Handler
{
    public override MsgType msgType { get { return MsgType.Start; } }

    public override Task<MyResponse> handle(object socket, object msg/* no use */)
    {
        this.baseScript.setState(ServerState.Starting);

        this.baseScript.listen(() => false);

        this.baseScript.setState(ServerState.Started);
        return Task.FromResult(new MyResponse(ECode.Success));
    }
}