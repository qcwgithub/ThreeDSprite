public class LocGetSummary : LocHandler {
    public override MsgType msgType { get { return MsgType.GetSummary; } }

    public async MyResponse handle(object socket, object msg) {
        this.logger.debug("LocGetSummary");

        var object res[] = [];

        ////
        res.push({
            workingDir: process.cwd(),
            purpose: Purpose[this.server.purpose],
            id: this.baseData.id,
            name: Utils.numberId2stringId(this.baseData.id),
            error: this.baseData.errorCount,
            uncauchtException: this.baseData.processData.uncaughtExceptionCount,
        });

        LocServerInfo arr[] = [];
        this.locData.map.forEach((info, _id, _) => {
            arr.push(info);
        });

        // 对所有服务器如送 MsgType.GetSummary
        for (int i = 0; i < arr.length; i++) {
            if (arr[i].id == ServerConst.WEB_ID) {
                continue;
            }

            var r = yield this.baseScript.sendYield(arr[i].socket, MsgType.GetSummary, {});
            if (r.err == ECode.Success) {
                var object infos[] = r.res;
                for (var j = 0; j < infos.length; j++) {
                    res.push(infos[j]);
                }
            } else {
                res.push({ id: arr[i].id, result: ECode[r.err] });
            }
        }
        return new MyResponse(ECode.Success, res);
    }
}