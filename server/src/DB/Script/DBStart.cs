// 文档：https://www.npmjs.com/package/mysql
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

public class DBStart : DBHandler
{
    public override MsgType msgType { get { return MsgType.Start; } }

    private void onConnectionStateChange(object sender, StateChangeEventArgs e)
    {
        this.server.logger.info("MySqlConnection StateChange %s -> %s", e.OriginalState.ToString(), e.CurrentState.ToString());
    }

    public override async Task<MyResponse> handle(object socket, string _msg/* no use */)
    {
        MyResponse r = null;
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        r = await this.baseScript.connectAsync(ServerConst.LOC_ID);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

        this.baseScript.listen(() => false);

        // this.dispatcher.dispatch(MsgType.DBStart, {}, this.utils.emptyReply);
        SqlConfig config = this.dbData.sqlConfig;
        this.dbData.connectionString = string.Format("server={0};user={1};database={2};password={3}",
            this.baseScript.myLoc().inIp, config.user, config.database);

        // https://dev.mysql.com/doc/connector-net/en/connector-net-connections-pooling.html
        // One approach that simplifies things is to avoid creating a MySqlConnection object manually

        // MySqlHelper
        this.dbData.mySqlConns = new List<MySqlConnection>();
        int connectedCount = 0;
        for (int i = 0; i < config.connectionLimit; i++)
        {
            var conn = new MySqlConnection(connStr);
            this.dbData.mySqlConns.Add(conn);
            conn.StateChange += (object sender, StateChangeEventArgs e) =>
            {
                if (e.CurrentState == ConnectionState.Open)
                {
                    connectedCount++;
                }
            };
            conn.OpenAsync();
        }

        while (connectedCount < config.connectionLimit)
        {
            await this.baseScript.waitYield(100);
        }

        // 把 TIMESTAMP DATETIME 转换为整数，毫秒

        this.baseScript.setState(ServerState.Started);
        return ECode.Success;
    }
}