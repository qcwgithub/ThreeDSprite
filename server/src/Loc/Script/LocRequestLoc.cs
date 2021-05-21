using System.Collections;
using System.Collections.Generic;

public class LocRequestLoc : LocHandler
{
    public override MsgType msgType { get { return MsgType.LocRequestLoc; } }

    public override IEnumerator handle(object socket, object _msg, MyResponse r)
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
            r.err = ECode.Success;
            yield break;
        }

        // 取到所有为止
        int index = 0;
        var res = new ResLocRequestLoc();
        res.locs = new List<Loc>();
        while (true)
        {
            int id = msg.ids[index];
            LocServerInfo info;
            if (!this.locData.map.TryGetValue(id, out info) || info == null)
            {
                yield return this.baseScript.waitYield(1000);
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

        r.err = ECode.Success;
        r.res = res;
    }
}