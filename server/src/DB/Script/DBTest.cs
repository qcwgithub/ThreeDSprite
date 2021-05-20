using System.Collections;

public class DBTest : DBHandler {
    public override MsgType msgType { get { return MsgType.DBTest; } }

    public override IEnumerator handle(object socket, object msg, MyResponse res) {
        // server.setTimerOnce(50, "DBQuery", {
        //     queryStr: "SELECT playerId FROM player_id;"
        // },
        // (object rep) => {
        //     server.logger.info("DBTest rep: " + JSON.stringify(rep));
        // });

        var a = msg;
        yield return this.baseScript.waitYield(1000);
        // a.triggerException();
        // reply(ECode.Success, "hello!!");
        // reply(ECode.Success, "world!!");
    }
}