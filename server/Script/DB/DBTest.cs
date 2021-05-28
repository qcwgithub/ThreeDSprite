using System.Collections;
using System.Threading.Tasks;

namespace Script
{
    public class DBTest : DBHandler
    {
        public override MsgType msgType { get { return MsgType.DBTest; } }

        public override async Task<MyResponse> handle(ISocket socket, string msg)
        {
            // server.setTimerOnce(50, "DBQuery", {
            //     queryStr: "SELECT playerId FROM player_id;"
            // },
            // (object rep) => {
            //     server.logger.info("DBTest rep: " + this.server.JSON.stringify(rep));
            // });

            var a = msg;
            await this.baseScript.waitAsync(1000);
            return new MyResponse(ECode.Success, null);
            // a.triggerException();
            // reply(ECode.Success, "hello!!");
            // reply(ECode.Success, "world!!");
        }
    }
}