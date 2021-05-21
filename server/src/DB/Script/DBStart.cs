// 文档：https://www.npmjs.com/package/mysql
using System.Collections;

public class DBStart : DBHandler {
    public override MsgType msgType { get { return MsgType.Start; } }

    public override IEnumerator handle(object socket, object _msg/* no use */, MyResponse r) {
        this.baseScript.setState(ServerState.Starting);

        // connect to loc
        yield return this.baseScript.connectYield(ServerConst.LOC_ID, true, r);
        this.baseData.locSocket = r.res;
        this.baseScript.setTimerLoop(1000, MsgType.KeepAliveToLoc, new object());

        this.baseScript.listen(() => false);

        // this.dispatcher.dispatch(MsgType.DBStart, {}, this.utils.emptyReply);
        _SqlConfig sql = this.dbData.sqlConfig;

        this.dbData.pool = mysql.createPool({
            connectionLimit: sql.connectionLimit,
            host: this.baseScript.myLoc().inIp,
            user: sql.user,
            password: sql.password,
            database: sql.database,
            typeCast: (field: mysql.UntypedFieldInfo & {
                string type;
                int length;
                string(): string;
                buffer(): Buffer;
                geometry(): null | mysql.GeometryType;
            }, next: () => any) => {
                var n = next();

                // 把 TIMESTAMP DATETIME 转换为整数，毫秒
                if (field.type == "TIMESTAMP" || field.type == "DATETIME") {
                    if (n == null) {
                        return 0;
                    }
                    // if (this.baseScript.isDevelopment()) {
                    //     if (n.constructor !== Date) {
                    //         this.logger.error("n.constructor !== Date");
                    //         return n;
                    //     }
                    // }
                    return n.getTime();
                }
                return n;
            },
        });

        this.dbData.pool.on("connection", conn => {
            this.logger.info("DB event: connection");
        });

        this.baseScript.setState(ServerState.Started);
        r.err = ECode.Success;
        yield break;
    }
}