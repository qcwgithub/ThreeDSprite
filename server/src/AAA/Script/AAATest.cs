
using System.Collections;
using System.Threading.Tasks;

public class AAATest : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAATest; } }

    private async Task<MyResponse> sub2()
    {
        this.logger.debug("AAATest, 3..." + this.sss);
        r = await this.baseScript.waitYield(10000);
        this.logger.debug("AAATest, 4..." + this.sss);
    }

    private async Task<MyResponse> sub1()
    {
        this.logger.debug("AAATest, 2..." + this.sss);
        r = await this.sub2();
        this.logger.debug("AAATest, 5..." + this.sss);
        r = await this.sub2();
        this.logger.debug("AAATest, 6..." + this.sss);
        r = await this.baseScript.waitYield(5000);
    }

    public bool ddd = true;
    public string sss = "NEW";
    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        if (!this.ddd)
        {
            yield break;
        }
        this.ddd = false;

        this.logger.debug("AAATest, 1..." + this.sss);
        var r = await this.sub1();
        this.logger.debug("AAATest, 7...!!???" + this.sss);

        // while (true) {
        //     var r = yield server.waitYield(2000);
        //     if (server.dbAccountSocket != null && server.networkHelper.isConnected(server.dbAccountSocket)) {   
        //         var test_r = yield server.sendYield(server.dbAccountSocket, MsgType.DBTest, {});
        //         server.logger.info("test_r: " + this.server.JSON.stringify(test_r));
        //     }
        // }

        // reply(ECode.Success);
    }
}