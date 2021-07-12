using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AskForStart : Handler<Server>
    {
        public override MsgType msgType { get { return MsgType.AskForStart; } }
        private int connectLocCount = 0;
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var data = this.server.data;
            if (data.grantedToStart)
            {
                this.server.proxyDispatch(null, MsgType.Start, null, null);
                return ECode.Success;
            }

            Console.WriteLine("**** {0} {1}", Utils.numberId2stringId(this.server.id), this.msgName);

            if (!data.timerSData.started)
            {
                data.timerSData.start();
            }

            var tcpClientScript = this.server.tcpClientScript;

            // 请求启动此进程，如果申请失败，则结束本进程
            TcpClientData locSocket;
            if (!data.otherServerSockets.TryGetValue(ServerConst.LOC_ID, out locSocket) ||
                tcpClientScript.isClosed(locSocket))
            {
                locSocket = new TcpClientData();
                Loc loc = this.server.getKnownLoc(ServerConst.LOC_ID);
                tcpClientScript.connectorConstructor(locSocket, loc.inIp, loc.inPort, this.server.data);
                data.otherServerSockets[ServerConst.LOC_ID] = locSocket;
            }

            bool connected = tcpClientScript.isConnected(locSocket);
            bool connecting = tcpClientScript.isConnecting(locSocket);
            bool isLocAndConnectLocFail = (data.id == ServerConst.LOC_ID && !connected && !connecting && connectLocCount > 0);
            if (!isLocAndConnectLocFail)
            {
                if (!connected)
                {
                    if (!connecting)
                    {
                        connectLocCount++;
                        // connect once
                        // this.server.logger.Info("call connect to loc");
                        this.server.tcpClientScript.connect(locSocket);
                        return ECode.Success;
                    }
                    return ECode.Success;
                }

                var r = await this.server.tcpClientScript.sendToServerAsync(
                        ServerConst.LOC_ID,
                        MsgType.LocReportLoc,
                        new MsgLocReportLoc { id = this.baseData.id, loc = this.server.myLoc() }
                    );

                if (r.err != ECode.Success)
                {
                    // console.error("!canStart: " + r.err);
                    // process.exit(1);
                    Console.WriteLine("**** {0} {1} error: {2}, exit now.", Utils.numberId2stringId(this.server.id), this.msgName, r.err);
                    Environment.Exit(0);
                    return ECode.Success;
                }
            }

            if (data.id == ServerConst.LOC_ID)
            {
                if (connected)
                {
                    tcpClientScript.close(locSocket, "loc dose not need to connect to loc");
                }
                data.otherServerSockets.Remove(data.id);
            }

            Console.WriteLine("**** {0} {1} OK!", Utils.numberId2stringId(this.server.id), this.msgName);
            data.grantedToStart = true;
            data.needReportToLoc = false;
            this.server.proxyDispatch(null, MsgType.Start, null, null);
            return ECode.Success;
        }

        public override MyResponse postHandle(object socket, object msg, MyResponse r)
        {
            if (!this.server.data.grantedToStart)
            {
                this.server.setTimer(1, this.msgType, null);
            }
            return r;
        }
    }
}