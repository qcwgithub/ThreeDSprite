using System.Collections.Generic;
using System.Threading.Tasks;

public class LocGetSummary : LocHandler {
    public override MsgType msgType { get { return MsgType.GetSummary; } }

    public override async Task<MyResponse> handle(object socket, object msg) {
        this.logger.debug("LocGetSummary");

        var res = new Dictionary<string, string>
        {
            { "workingDir", process.cwd() },
            { "purpose", this.server.purpose.ToString() },
            { "id", this.baseData.id.ToString() },
            { "name", Utils.numberId2stringId(this.baseData.id) },
            { "error", this.baseData.errorCount.ToString() },
            { "uncauchtException", this.baseData.processData.uncaughtExceptionCount.ToString() },
        };

        List<LocServerInfo> list = new List<LocServerInfo>();
        foreach (var kv in this.locData.map)
        {
            list.Add(kv.Value);
        };

        // 对所有服务器如送 MsgType.GetSummary
        // for (int i = 0; i < list.Count; i++) {
        //     if (list[i].id == ServerConst.WEB_ID) {
        //         continue;
        //     }

        //     var r = await this.baseScript.sendYield(list[i].socket, MsgType.GetSummary, new object());
        //     if (r.err == ECode.Success) {
        //         object[] infos[] = r.res;
        //         for (var j = 0; j < infos.length; j++) {
        //             res.push(infos[j]);
        //         }
        //     } else {
        //         res.push({ id: arr[i].id, result: ECode[r.err] });
        //     }
        // }
        return new MyResponse(ECode.Success, res);
    }
}