public class DBTest : DBHandler {
    public override MsgType msgType { get { return MsgType.DBTest; } }

    public async MyResponse handle(object socket, object msg) {
        // server.setTimerOnce(50, "DBQuery", {
        //     queryStr: "SELECT playerId FROM player_id;"
        // },
        // (object rep) => {
        //     server.logger.info("DBTest rep: " + JSON.stringify(rep));
        // });

        var a = msg;
        return await this.baseScript.waitYield(1000);
        // a.triggerException();
        // reply(ECode.Success, "hello!!");
        // reply(ECode.Success, "world!!");
    }
}