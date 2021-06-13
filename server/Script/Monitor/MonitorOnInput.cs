using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Script
{
    public class MonitorOnInput : MonitorHandler
    {
        public override MsgType msgType => MsgType.MonitorOnInput;

        private void broadcast(MsgLocBroadcast msg)
        {
            int selfIndex = msg.ids.IndexOf(ServerConst.MONITOR_ID);
            if (selfIndex >= 0)
            {
                msg.ids.RemoveAt(selfIndex);
            }

            if (this.server.tcpClientScript.isServerConnected(ServerConst.LOC_ID))
            {
                this.server.logger.Info("broadcast!");
                this.server.tcpClientScript.sendToServer(
                    ServerConst.LOC_ID,
                    MsgType.Broadcast,
                    msg, null);
            }
            else
            {
                if (msg.ids.Count > 0)
                {
                    this.server.logger.Info("broadcast failed, loc is not connected");
                }
            }

            // 自己
            if (selfIndex >= 0)
            {
                this.server.proxyDispatch(null, msg.msgType, msg.getMsg(), null);
            }
        }

        const string RELOAD_SCRIPT = "reloadScript";
        const string SHUT_DOWN = "shutDown";
        const string PM_ACTION = "pmAction";
        const string AAA_ACTION = "aaaAction";

        string lastContent = "";
        public override async Task<MyResponse> handle(TcpClientData socket/* null */, object _msg/* null */)
        {
            int tryCount = 0;
            string content = string.Empty;
            bool readSuccess = false;
            while (true)
            {
                await this.server.waitAsync(500);
                try
                {
                    content = File.ReadAllText(this.server.monitorData.inputFileName);
                    readSuccess = true;
                    break;
                }
                catch
                {
                    tryCount++;
                    if (tryCount >= 5)
                    {
                        break;
                    }
                }
            }

            if (!readSuccess)
            {
                this.server.logger.Info("!readSuccess");
                return ECode.Success;
            }

            if (content == this.lastContent)
            {
                // no log
                return ECode.Success;
            }

            if (string.IsNullOrEmpty(content))
            {
                this.server.logger.Info("input.txt content is null or empty");
                return ECode.Success;
            }

            this.lastContent = content;

            // 某一行以 // 开始，则他后面的行全部忽略
            List<string> lines = content.Split("\r\n".ToCharArray()).ToList();
            int index = lines.FindIndex(line => line.StartsWith("//"));
            if (index >= 0)
            {
                lines.RemoveRange(index, lines.Count - index);
            }
            //.Where(line => line.Length > 0 && !line.StartsWith("//")).ToList();
            if (lines.Count == 0)
            {
                this.server.logger.Info("lines.Count == 0");
                return ECode.Success;
            }

            // 每一行如果包含 // ，则这行后面的内容忽略
            // lines.Select(line => 
            // {
            //     int comment = line.IndexOf("//");
            //     if (comment >= 0)
            //         return line.Substring(comment + 2);
            //     return line;
            // });

            string line0 = lines[0];
            int space1 = line0.IndexOf(' ', 0);
            int space2 = line0.IndexOf(' ', space1 + 1);

            string op = line0.Substring(0, space1);
            List<int> ids = null;
            if (space2 > 0)
            {
                ids = this.server.JSON.parse<List<int>>(line0.Substring(space1 + 1, space2 - space1 - 1));
            }
            else
            {
                ids = this.server.JSON.parse<List<int>>(line0.Substring(space1 + 1));
            }

            string param = null;
            if (space2 > 0)
            {
                lines[0] = line0.Substring(space2 + 1);
                param = string.Join('\n', lines.ToArray());
            }

            switch (op)
            {
                case RELOAD_SCRIPT:
                    {
                        this.broadcast(new MsgLocBroadcast { ids = ids, msgType = MsgType.ReloadScript });
                    }
                    break;

                case SHUT_DOWN:
                    {
                        this.broadcast(new MsgLocBroadcast { ids = ids, msgType = MsgType.Shutdown });
                    }
                    break;

                case PM_ACTION:
                    {
                        var msg = this.server.JSON.parse<MsgPMAction>(param);
                        this.broadcast(new MsgLocBroadcastMsgPMAction { ids = ids, msgType = MsgType.ServerAction, msg = msg });
                    }
                    break;

                case AAA_ACTION:
                    {
                        var msg = this.server.JSON.parse<MsgAAAAction>(param);
                        this.broadcast(new MsgLocBroadcastMsgAAAAction { ids = ids, msgType = MsgType.ServerAction, msg = msg });
                    }
                    break;
                default:
                    {
                        this.server.logger.Info("unknown operation: " + op);
                    }
                    break;
            }

            return ECode.Success;
        }
    }
}