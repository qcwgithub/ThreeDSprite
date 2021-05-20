
public class OnRunScript : Handler {
    public override MsgType msgType { get { return MsgType.RunScript; } }

    public override MyResponse handle(object socket, object msg) {
        // this.logger.warn("******** %s ********", this.msgName);

        // var S = "[function(server){";
        // var E = "}]";
        // if (!msg.script.startsWith(S)) {
        //     msg.script = S + msg.script + E;
        //     // this.logger.warn("%s auto wrap %s...%s", this.msgName, S, E);
        // }

        // var fun: (server: Server) => void = eval(msg.script)[0];
        // try {
        //     fun(this.server);
        //     r.err = ECode.Success;
        yield break;
        // }
        // catch (ex) {
        //     this.baseScript.error(ex.toString());
        //     return MyResponse.create(ECode.Exception);
        // }
    }
}