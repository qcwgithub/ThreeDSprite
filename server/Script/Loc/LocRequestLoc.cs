using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class LocRequestLoc : LocHandler
    {
        public override MsgType msgType { get { return MsgType.LocRequestLoc; } }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgLocRequestLoc>(_msg);
            this.logger.Info("LocRequestLoc ids: " + JsonUtils.stringify(msg.serverIds));

            if (msg.serverIds == null)
            {
                msg.serverIds = new List<int>();
                foreach (var kv in this.locData.map)
                {
                    msg.serverIds.Add(kv.Key);
                }
            }

            if (msg.serverIds.Count == 0)
            {
                return ECode.Success;
            }

            // 取到所有为止
            int index = 0;
            var res = new ResLocRequestLoc();
            res.locs = new List<Loc>();
            while (true)
            {
                int serverId = msg.serverIds[index];
                LocServerInfo info;
                if (!this.locData.map.TryGetValue(serverId, out info))
                {
                    await this.server.waitAsync(1000);
                }
                else
                {
                    res.locs.Add(info.loc);
                    index++;
                    if (index == msg.serverIds.Count)
                    {
                        break;
                    }
                }
            }

            return new MyResponse(ECode.Success, res);
        }
    }
}