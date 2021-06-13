using System;
using System.Linq;
using System.Net.Sockets;
using Data;

namespace Script
{
    // Server 提供给 IScript 数据、其他脚本的访问
    public abstract partial class Server
    {
        public int setTimer(int timeoutMs, MsgType msgType, object msg/*, bool loop*/)
        {
            int id = this.data.timerData.setTimer(timeoutMs, msgType, msg/*, loop*/);
            return id;
        }

        public void clearTimer(int id)
        {
            this.data.timerData.clearTimer(id);
        }
    }
}