using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LocRequestLoc : LocHandler
{
    public override MsgType msgType { get { return MsgType.LocRequestLoc; } }

    public override async Task<MyResponse> handle(object socket, object _msg)
    {
        var msg = _msg as MsgLocRequestLoc;
        this.logger.info("LocRequestConfig ids: " + this.server.JSON.stringify(msg.ids));

        if (msg.ids == null)
        {
            msg.ids = new List<int>();
            foreach (var kv in this.locData.map)
            {
                msg.ids.Add(kv.Key);
            }
        }

        if (msg.ids.Count == 0)
        {
            return ECode.Success;
        }

        // 取到所有为止
        int index = 0;
        var res = new ResLocRequestLoc();
        res.locs = new List<Loc>();
        while (true)
        {
            int id = msg.ids[index];
            LocServerInfo info;
            if (!this.locData.map.TryGetValue(id, out info))
            {
                await this.baseScript.waitYield(1000);
            }
            else
            {
                res.locs.Add(info.loc);
                index++;
                if (index == msg.ids.Count)
                {
                    break;
                }
            }
        }

        return new MyResponse(ECode.Success, res);
    }
}