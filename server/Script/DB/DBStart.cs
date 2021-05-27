// 文档：https://www.npmjs.com/package/mysql
using System.Data;
using System.Threading.Tasks;
using Data;

public class DBStart : DBHandler
{
    public override MsgType msgType { get { return MsgType.Start; } }

    private void onConnectionStateChange(object sender, StateChangeEventArgs e)
    {
        this.server.logger.InfoFormat("MySqlConnection StateChange {0} -> {1}", e.OriginalState, e.CurrentState);
    }

    public override async Task<MyResponse> handle(ISocket socket, string _msg/* no use */)
    {
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        this.baseData.locSocket = await this.baseScript.connectAsync(ServerConst.LOC_ID);
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

        this.baseScript.listen(() => false);

        // this.dispatcher.dispatch(MsgType.DBStart, {}, this.utils.emptyReply);
        SqlConfig config = this.dbData.sqlConfig;
        this.dbData.connectionString = string.Format("server={0};user={1};database={2};password={3}",
            this.baseScript.myLoc().inIp, config.user, config.database, config.password);

        // 把 TIMESTAMP DATETIME 转换为整数，毫秒

        this.baseScript.setState(ServerState.Started);
        return ECode.Success;
    }
}