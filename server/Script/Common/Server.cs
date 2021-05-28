using System;
using System.Linq;
using Data;

namespace Script
{
    // Server 提供给 IScript 数据、其他脚本的访问
    public abstract class Server
    {
        public GlobalData globalData;
        public abstract BaseData baseData { get; }

        public log4net.ILog logger {
            get {
                return this.baseData.logger;
            }
        }

        public BaseScript baseScript;
        public INetProto serverNetwork;

        public MessageDispatcher dispatcher;
        public SqlLog sqlLog;
        public Utils utils;
        public JsonUtils JSON;
        public TimerScript timerScript;
    }
}
