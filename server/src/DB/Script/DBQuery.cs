public class DBQuery : DBHandler {
    public override MsgType msgType { get { return MsgType.DBQuery; } }

    public async MyResponse handle(object socket, MsgDBQuery msg) {
        this.logger.debug("DBQuery: " + msg.queryStr);

        this.dbData.queryCount++;

        if (msg.valueTypes != null) {
            for (var key in msg.valueTypes) {
                var index = parseInt(key);
                if (!(index >= 0 && index < msg.values.length)) {
                    return MyResponse.create(ECode.DBQueryValueTypesError);
                }
                MyDBValueType type = msg.valueTypes[key];
                switch (type) {
                    case MyDBValueType.DateTime:
                        msg.values[index] = new Date(msg.values[index]);
                        break;
                    default:
                        return MyResponse.create(ECode.DBQueryValueTypesError);
                }
            }
        }

        var data = this.dbData;
        var waiter = new WaitCallBack().init(() => {
            var cb = (error: mysql.MysqlError, object result/*, object fields*/) => {
                if (error) {
                    this.baseScript.error("DBQuery error: " + JSON.stringify(error));
                   var res = new ResMysqlError { code = error.code, errno = error.errno };
                    waiter.finish(new MyResponse(ECode.SqlError, res));
                }
                else {
                    if (msg.expectedAffectedRows >= 0) {
                        if (result == null) {
                            this.baseScript.error("expectedAffectedRows: %d, result==null, queryStr: %s", msg.expectedAffectedRows, msg.queryStr);
                        }
                        else {
                            var pkt = result as mysql.OkPacket;
                            if (pkt.affectedRows != msg.expectedAffectedRows) {
                                this.baseScript.error("expectedAffectedRows: %d, okPackage.affectedRows=%s, queryStr: %s",
                                    msg.expectedAffectedRows,
                                    (pkt.affectedRows == null ? "null" : pkt.affectedRows.toString()),
                                    msg.queryStr);
                            }
                        }
                    }
                    waiter.finish(new MyResponse(ECode.Success, result));
                }
            };

            if (msg.values == null) {
                data.pool.query(msg.queryStr, cb);
            }
            else {
                data.pool.query(msg.queryStr, msg.values, cb);
            }
        });

        return yield waiter;
    }
}