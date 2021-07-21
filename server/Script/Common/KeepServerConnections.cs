using Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Script
{
    // 连接的发起方，保持连接。每隔一段时间检查一下
    public class KeepServerConnections : Handler<Server>
    {
        public override MsgType msgType => MsgType.KeepServerConnections;

        // ids=null 表示全部，monitor使用
        public async Task<MyResponse> requestLocAsync(List<int> ids)
        {
            this.logger.Info("requestLoc " + JsonUtils.stringify(ids));
            var r = await this.server.tcpClientScript.sendToServerAsync(ServerConst.LOC_ID,
                    MsgType.LocRequestLoc,
                    new MsgLocRequestLoc { serverIds = ids });

            if (r.err != ECode.Success)
            {
                this.logger.Info("requestLoc error " + r.err);
                return r.err;
            }

            var res = this.server.CastObject<ResLocRequestLoc>(r.res);
            List<Loc> locs = res.locs;
            for (int i = 0; i < locs.Count; i++)
            {
                this.server.addKnownLoc(locs[i]);
            }
            this.logger.Info("requestLoc OK");
            return ECode.Success;
        }

        public override async Task<MyResponse> handle(TcpClientData _socket/* null */, object _msg/* null */)
        {
            var data = this.server.data;
            var tcpClientScript = this.server.tcpClientScript;

            if (data.connectToServerIds.Contains(ServerConst.LOC_ID))
            {
                TcpClientData locSocket;
                if (!data.otherServerSockets.TryGetValue(ServerConst.LOC_ID, out locSocket) ||
                    locSocket.isClosed())
                {
                    locSocket = new TcpClientData();
                    Loc loc = this.server.getKnownLoc(ServerConst.LOC_ID);
                    locSocket.connectorInit(this.server.data.tcpClientCallback, loc.inIp, loc.inPort);
                    data.otherServerSockets[ServerConst.LOC_ID] = locSocket;
                }

                if (!locSocket.isConnected())
                {
                    if (!locSocket.isConnecting())
                    {
                        // connect once
                        // this.server.logger.Info("call connect to loc");
                        locSocket.connect();
                        return ECode.Success;
                    }
                    return ECode.Success;
                }
            }

            // check missing loc
            List<int> missingIds = null;
            for (int i = 0; i < data.connectToServerIds.Count; i++)
            {
                int serverId = data.connectToServerIds[i];
                if (this.server.getKnownLoc(serverId) == null)
                {
                    if (missingIds == null) missingIds = new List<int>();
                    missingIds.Add(serverId);
                }
            }

            if (missingIds != null)
            {
                var r = await this.requestLocAsync(missingIds);
                if (r.err != ECode.Success)
                {
                    return r.err;
                }
            }

            // check connection
            for (int i = 0; i < data.connectToServerIds.Count; i++)
            {
                int serverId = data.connectToServerIds[i];
                TcpClientData socket;
                if (!data.otherServerSockets.TryGetValue(serverId, out socket) ||
                    socket.isClosed())
                {
                    socket = new TcpClientData();
                    Loc loc = this.server.getKnownLoc(serverId);
                    socket.connectorInit(this.server.data.tcpClientCallback, loc.inIp, loc.inPort);
                    data.otherServerSockets[serverId] = socket;
                }
                if (!socket.isConnected() && !socket.isConnecting())
                {
                    // connect once
                    // this.server.logger.Info("call connect to " + serverId + ", " + this.server.data.getInt("keepServerConnectionsing"));
                    socket.connect();
                }
            }

            return ECode.Success;
        }

        public override MyResponse postHandle(object socket, object msg, MyResponse r)
        {
            this.server.setTimer(3, this.msgType, null);
            return r;
        }
    }
}