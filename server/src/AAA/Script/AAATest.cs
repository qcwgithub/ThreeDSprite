
using System.Collections;

public class AAATest : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAATest; } }

    private IEnumerator sub2()
    {
        this.logger.debug("AAATest, 3..." + this.sss);
        yield return this.baseScript.waitYield(10000);
        this.logger.debug("AAATest, 4..." + this.sss);
    }

    private IEnumerator sub1()
    {
        this.logger.debug("AAATest, 2..." + this.sss);
        yield return this.sub2();
        this.logger.debug("AAATest, 5..." + this.sss);
        yield return this.sub2();
        this.logger.debug("AAATest, 6..." + this.sss);
        yield return this.baseScript.waitYield(5000);
    }

    public bool ddd = true;
    public string sss = "NEW";
    public override IEnumerator handle(object socket, object _msg, MyResponse r)
    {
        if (!this.ddd)
        {
            yield break;
        }
        this.ddd = false;

        this.logger.debug("AAATest, 1..." + this.sss);
        yield return this.sub1();
        this.logger.debug("AAATest, 7...!!???" + this.sss);

        // while (true) {
        //     var r = yield server.waitYield(2000);
        //     if (server.dbAccountSocket != null && server.networkHelper.isConnected(server.dbAccountSocket)) {   
        //         var test_r = yield server.sendYield(server.dbAccountSocket, MsgType.DBTest, {});
        //         server.logger.info("test_r: " + JSON.stringify(test_r));
        //     }
        // }

        // reply(ECode.Success);
    }
}