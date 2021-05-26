
using System.Collections;
using System.Threading.Tasks;

public class AAATest : AAAHandler
{
    public override MsgType msgType { get { return MsgType.AAATest; } }

    private async Task<MyResponse> sub2()
    {
        this.logger.debug("AAATest, 3..." + this.sss);
        await this.baseScript.waitAsync(10000);
        this.logger.debug("AAATest, 4..." + this.sss);
        return ECode.Success;
    }

    private async Task<MyResponse> sub1()
    {
        this.logger.debug("AAATest, 2..." + this.sss);
        var r = await this.sub2();
        this.logger.debug("AAATest, 5..." + this.sss);
        r = await this.sub2();
        this.logger.debug("AAATest, 6..." + this.sss);
        await this.baseScript.waitAsync(5000);
        return ECode.Success;
    }

    public bool ddd = true;
    public string sss = "NEW";
    public override async Task<MyResponse> handle(ISocket socket, string _msg)
    {
        this.ddd = false;

        this.logger.debug("AAATest, 1..." + this.sss);
        var r = await this.sub1();
        this.logger.debug("AAATest, 7...!!???" + this.sss);
        return ECode.Success;

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