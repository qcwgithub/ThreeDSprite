using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class DBGetSummary : DBHandler
    {
        public override MsgType msgType { get { return MsgType.GetSummary; } }

        public override Task<MyResponse> handle(TcpClientData socket, string _msg)
        {
            this.logger.Debug("DBGetSummary");
            return Task.FromResult(new MyResponse(ECode.Success, null));

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
}