using System;
using System.Collections.Generic;
using Data;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Script
{
    // 初始化 DataEntry 里的数据
    public class DataCreation
    {
        void InitBaseData(ServerData data, int serverId, List<int> connectToServerIds)
        {
            data.id = serverId;

            data.knownLocs[this.dataEntry.locLoc.id] = dataEntry.locLoc;
            
            var selfLoc = new Loc
            {
                id = serverId,
                inIp = this.dataEntry.thisMachineConfig.inIp,
                outIp = this.dataEntry.thisMachineConfig.outIp,
                outDomain = this.dataEntry.thisMachineConfig.outDomain,
                inPort = ServerConst.getInPortByServerId(serverId),
                outPort = ServerConst.getOutPortByServerId(serverId),
            };
            data.knownLocs[selfLoc.id] = selfLoc;

            data.logger = this.log4NetCreation.getLogger(Utils.numberId2stringId(serverId));
            data.timerData = new Data.TimerData { serverData = data };

            data.connectToServerIds.AddRange(connectToServerIds);
        }
        void InitListenForServer(ServerData data)
        {
            data.tcpListenerForServer = new TcpListenerData() { isForClient = false, serverData = data };
        }
        void InitListenForClient(ServerData data)
        {
            data.tcpListenerForClient = new TcpListenerData() { isForClient = true, serverData = data };
        }

        LocData CreateLocData(int id)
        {
            var data = new LocData();
            InitBaseData(data, id, new List<int>());
            InitListenForServer(data);
            return data;
        }

        AAAData CreateAAAData(int id)
        {
            var data = new AAAData();
            InitBaseData(data, id, new List<int>
            {
                ServerConst.LOC_ID,
                ServerConst.DB_ACCOUNT_ID,
                ServerConst.DB_PLAYER_ID,
                ServerConst.DB_LOG_ID
            });

            
            InitListenForServer(data);
            InitListenForClient(data);
            // data.timerData.setTimer(5000, MsgType.ReloadScript, null, true);
            return data;
        }

        DBData CreateDBData(int id)
        {
            var data = new DBData();
            InitBaseData(data, id, new List<int> { ServerConst.LOC_ID });
            if (id == ServerConst.DB_ACCOUNT_ID)
            {
                data.sqlConfig = this.configLoader.AccountSqlConfig;
            }
            else if (id == ServerConst.DB_PLAYER_ID)
            {
                data.sqlConfig = this.configLoader.PlayerSqlConfig;
            }
            else
            {
                data.sqlConfig = this.configLoader.LogSqlConfig;
            }

            data.connectionString = string.Format("server={0};user={1};database={2};password={3}",
                data.knownLocs[data.id].inIp,
                data.sqlConfig.user,
                data.sqlConfig.database,
                data.sqlConfig.password);

            InitListenForServer(data);

            return data;
        }

        PMData CreatePMData(int id)
        {
            var data = new PMData();
            InitBaseData(data, id, new List<int> {
                ServerConst.LOC_ID,
                ServerConst.AAA_ID,
                ServerConst.DB_PLAYER_ID,
                ServerConst.DB_LOG_ID
            });

            // data.timerData.setTimer(1, MsgType.PMKeepAliveToAAA, null, false);
            InitListenForServer(data);
            InitListenForClient(data);
            return data;
        }

        MonitorData CreateMonitorData(int id)
        {
            var data = new MonitorData();
            InitBaseData(data, id, new List<int> { ServerConst.LOC_ID });

            // start watch file
            data.inputFileName = @"./input/input.txt";
            var watcher = data.watcher = new FileSystemWatcher(@"./input");
            watcher.Filter = "input.txt";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watcher.Changed += data.OnChanged;

            return data;
        }

        ServerData CreateServerData(int id)
        {
            if (id == ServerConst.LOC_ID)
            {
                return CreateLocData(id);
            }
            else if (id == ServerConst.AAA_ID)
            {
                return CreateAAAData(id);
            }
            else if (id == ServerConst.WEB_ID)
            {

            }
            else if (id == ServerConst.MONITOR_ID)
            {
                return CreateMonitorData(id);
            }
            else if (id == ServerConst.DB_ACCOUNT_ID ||
                id == ServerConst.DB_PLAYER_ID ||
                id == ServerConst.DB_LOG_ID)
            {
                return CreateDBData(id);
            }
            else if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID)
            {
                return CreatePMData(id);
            }

            return null;
        }

        DataEntry dataEntry;
        ConfigLoader configLoader;
        Log4netCreation log4NetCreation;
        public void Create(DataEntry dataEntry, ConfigLoader configLoader, List<int> serverIds, Purpose purpose, Log4netCreation log4NetCreation)
        {
            this.dataEntry = dataEntry;
            this.configLoader = configLoader;
            this.log4NetCreation = log4NetCreation;

            dataEntry.processData = new ProcessData();
            dataEntry.serverIds = serverIds;
            dataEntry.purpose = purpose;
            dataEntry.timezoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
            ServerConst.initPorts(dataEntry.purpose);

            var versionConfig = configLoader.VersionConfig;
            dataEntry.androidVersion = versionConfig.android;
            dataEntry.iOSVersion = versionConfig.ios;


            dataEntry.thisMachineConfig = configLoader.ThisMachineConfig;
            dataEntry.locLoc = new Loc()
            {
                id = ServerConst.LOC_ID,
                inIp = configLoader.LocConfig.host,
                outIp = null,
                outDomain = null,
                inPort = ServerConst.LOC_PORT,
            };

            dataEntry.name2Type = new Dictionary<string, Type>();
            // system types
            dataEntry.name2Type.Add(typeof(int).Name, typeof(int));
            dataEntry.name2Type.Add(typeof(string).Name, typeof(string));
            //dataEntry.name2Type.Add(typeof(List<int>).Name, typeof(List<int>));
            //dataEntry.name2Type.Add(typeof(List<string>).Name, typeof(List<string>));

            // data.dll types
            var allDataTypes = dataEntry.GetType().Assembly.GetTypes();
            foreach (var type in allDataTypes)
            {
                if (typeof(ISerializable).IsAssignableFrom(type))
                {
                    dataEntry.name2Type.Add(type.Name, type);
                }
            }


            //// per-server data
            dataEntry.serverDatas = new Dictionary<int, ServerData>();
            foreach (var serverId in dataEntry.serverIds)
            {
                dataEntry.serverDatas.Add(serverId, this.CreateServerData(serverId));
            }
        }
    }
}