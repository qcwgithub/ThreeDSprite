using System.Collections;

public class DBGetSummary : DBHandler
{
    public override MsgType msgType { get { return MsgType.GetSummary; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        this.logger.debug("DBGetSummary");
        yield break;

        // var info = {
        //     workingDir: process.cwd(),
        //     purpose: Purpose[this.server.purpose],
        //     id: this.baseData.id,
        //     name: Utils.numberId2stringId(this.baseData.id),
        //     queryCount: this.dbData.queryCount,
        //     error: this.baseData.errorCount,
        //     uncauchtException: this.baseData.processData.uncaughtExceptionCount,
        // };
        // return new MyResponse(ECode.Success, [info]);
    }
}