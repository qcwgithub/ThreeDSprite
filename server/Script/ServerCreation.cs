using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using log4net.Repository;
using log4net.Appender;
using System.Xml;
using System.Xml.Linq;
using Data;

namespace Script
{
    public class ServerCreation
    {
        void InitBase<T>(T s) where T : Server
        {
            s.baseScript = new BaseScript { server = s };
            s.serverNetwork = new NetProtoTcp { server = s };
            s.timerScript = new TimerScript { server = s };
            s.dispatcher = new MessageDispatcher { server = s };
            s.utils = new Utils();
            s.JSON = new JsonUtils();
            s.sqlLog = new SqlLog { server = s };

            s.dispatcher.addHandler(new OnShutdown<T> { server = s });
            // server.dispatcher.addHandler(new OnReloadScript());
            // server.dispatcher.addHandler(new OnRunScript());
            s.dispatcher.addHandler(new OnConnect<T> { server = s });
            s.dispatcher.addHandler(new OnDisconnect<T> { server = s });
            s.dispatcher.addHandler(new KeepAliveToLoc<T> { server = s });
        }

        GlobalData global;
        public List<Server> Create(Data.GlobalData data)
        {
            this.global = data;
            var list = new List<Server>();
            for (int i = 0; i < data.serverIds.Count; i++)
            {
                int id = data.serverIds[i];

                if (id == ServerConst.LOC_ID)
                {
                    var s = new LocServer();
                    s.locData = (LocData)global.serverDatas[id];
                    
                    this.InitBase(s);

                    s.locScript = new LocScript { server = s };
                    s.dispatcher.addHandler(new LocStart { server = s });
                    s.dispatcher.addHandler(new LocOnDisconnect { server = s });
                    s.dispatcher.addHandler(new LocReportLoc { server = s });
                    s.dispatcher.addHandler(new LocRequestLoc { server = s });
                    s.dispatcher.addHandler(new LocBroadcast { server = s });
                    s.dispatcher.addHandler(new LocGetSummary { server = s });
                    list.Add(s);
                }
                else if (id == ServerConst.AAA_ID)
                {
                    var s = new AAAServer();
                    s.aaaData = (AAAData)global.serverDatas[id];

                    this.InitBase(s);

                    s.aaaScript = new AAAScript { server = s };
                    s.aaaSqlUtils = new AAASqlUtils { server = s };
                    s.channelUuid = new AAAChannel_Uuid { server = s };
                    s.channelDebug = new AAAChannel_Debug { server = s };
                    s.channelApple = new AAAChannel_Apple { server = s };
                    s.channelIvy = new AAAChannel_Ivy { server = s };

                    s.dispatcher.addHandler(new AAAStart { server = s });
                    s.dispatcher.addHandler(new AAATest { server = s });
                    s.dispatcher.addHandler(new AAAOnPMAlive { server = s });
                    s.dispatcher.addHandler(new AAALoadPlayerId { server = s });
                    s.dispatcher.addHandler(new AAAChangeChannel { server = s });
                    s.dispatcher.addHandler(new AAAPlayerLogin { server = s });
                    s.dispatcher.addHandler(new AAADestroyPlayer { server = s });
                    s.dispatcher.addHandler(new AAAGetSummary { server = s });
                    s.dispatcher.addHandler(new AAAShutdown { server = s });
                    s.dispatcher.addHandler(new AAAAction { server = s });

                    list.Add(s);
                }
                else if (id == ServerConst.WEB_ID)
                {
                    continue;
                }
                else if (id == ServerConst.DB_ACCOUNT_ID)
                {
                    var s = new DBServer();
                    s.dbData = (DBData)global.serverDatas[id];

                    this.InitBase(s);

                    s.dbScript = new DBScript { server = s };
                    s.dispatcher.addHandler(new DBStart { server = s });
                    s.dispatcher.addHandler(new DBQuery { server = s });
                    s.dispatcher.addHandler(new DBTest { server = s });
                    s.dispatcher.addHandler(new DBGetSummary { server = s });
                    list.Add(s);
                }
                else if (id == ServerConst.DB_PLAYER_ID)
                {
                    var s = new DBServer();
                    s.dbData = (DBData)global.serverDatas[id];

                    this.InitBase(s);

                    s.dbScript = new DBScript { server = s };
                    s.dispatcher.addHandler(new DBStart { server = s });
                    s.dispatcher.addHandler(new DBQuery { server = s });
                    s.dispatcher.addHandler(new DBTest { server = s });
                    s.dispatcher.addHandler(new DBGetSummary { server = s });
                    list.Add(s);
                }
                else if (id == ServerConst.DB_LOG_ID)
                {
                    var s = new DBServer();
                    s.dbData = (DBData)global.serverDatas[id];

                    this.InitBase(s);

                    s.dbScript = new DBScript { server = s };
                    s.dispatcher.addHandler(new DBStart { server = s });
                    s.dispatcher.addHandler(new DBQuery { server = s });
                    s.dispatcher.addHandler(new DBTest { server = s });
                    s.dispatcher.addHandler(new DBGetSummary { server = s });
                    list.Add(s);
                }
                else if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID)
                {
                    var s = new PMServer();
                    s.pmData = (PMData)global.serverDatas[id];

                    this.InitBase(s);

                    s.pmScript = new PMScript { server = s };
                    s.pmSqlUtils = new PMSqlUtils { server = s };
                    s.pmPlayerToSqlTablePlayer = new PMPlayerToSqlTablePlayer { server = s };
                    s.pmScriptCreateNewPlayer = new PMScriptCreateNewPlayer { server = s };

                    s.gameScript = new GameScriptServer();
                    s.gameScript.init(s.pmData, s);

                    s.scUtils = new SCUtils();
                    s.scUtils.init(s.pmData, s);

                    s.dispatcher.addHandler(new PMStart { server = s });
                    s.dispatcher.addHandler(new PMOnDisconnect { server = s });
                    s.dispatcher.addHandler(new PMKeepAliveToAAA { server = s });
                    s.dispatcher.addHandler(new PMPlayerLogin { server = s });
                    s.dispatcher.addHandler(new PMChangeChannel { server = s });
                    s.dispatcher.addHandler(new PMPreparePlayerLogin { server = s });
                    s.dispatcher.addHandler(new PMDestroyPlayer { server = s });
                    s.dispatcher.addHandler(new PMAction { server = s });

                    list.Add(s);
                }
                else
                {
                    continue;
                }
            }
            return list;
        }
    }
}