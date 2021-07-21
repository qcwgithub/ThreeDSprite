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
            data.serverId = serverId;

            data.knownLocs[this.dataEntry.locLoc.serverId] = dataEntry.locLoc;
            
            var selfLoc = new Loc
            {
                serverId = serverId,
                inIp = this.dataEntry.thisMachineConfig.inIp,
                outIp = this.dataEntry.thisMachineConfig.outIp,
                outDomain = this.dataEntry.thisMachineConfig.outDomain,
                inPort = ServerConst.getInPortByServerId(serverId),
                outPort = ServerConst.getOutPortByServerId(serverId),
            };
            data.knownLocs[selfLoc.serverId] = selfLoc;

            data.logger = this.log4NetCreation.getLogger(Utils.numberId2stringId(serverId));
            data.timerSData = new Data.TimerSData { serverData = data };

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

        LocData CreateLocData(int serverId)
        {
            var data = new LocData();
            InitBaseData(data, serverId, new List<int>());
            InitListenForServer(data);
            return data;
        }

        AAAData CreateAAAData(int serverId)
        {
            var data = new AAAData();
            InitBaseData(data, serverId, new List<int>
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

        DBData CreateDBData(int serverId)
        {
            var data = new DBData();
            InitBaseData(data, serverId, new List<int> { ServerConst.LOC_ID });
            if (serverId == ServerConst.DB_ACCOUNT_ID)
            {
                data.sqlConfig = this.configLoader.AccountSqlConfig;
            }
            else if (serverId == ServerConst.DB_PLAYER_ID)
            {
                data.sqlConfig = this.configLoader.PlayerSqlConfig;
            }
            else
            {
                data.sqlConfig = this.configLoader.LogSqlConfig;
            }

            data.connectionString = string.Format("server={0};user={1};database={2};password={3}",
                data.knownLocs[data.serverId].inIp,
                data.sqlConfig.user,
                data.sqlConfig.database,
                data.sqlConfig.password);

            InitListenForServer(data);

            return data;
        }

        PMData CreatePMData(int serverId)
        {
            var data = new PMData();
            InitBaseData(data, serverId, new List<int> {
                ServerConst.LOC_ID,
                ServerConst.AAA_ID,
                ServerConst.DB_PLAYER_ID,
                ServerConst.DB_LOG_ID,
                ServerConst.LOBBY_ID,
            });

            ConfigScript.Load(data, file => this.configLoader.loadGameText(file));

            // data.timerData.setTimer(1, MsgType.PMKeepAliveToAAA, null, false);
            InitListenForServer(data);
            InitListenForClient(data);
            return data;
        }

        LobbyData CreateLobbyData(int serverId)
        {
            var data = new LobbyData();
            InitBaseData(data, serverId, new List<int>());
            InitListenForServer(data);
            return data;
        }

        BMData CreateBMData(int serverId)
        {
            var data = new BMData();
            InitBaseData(data, serverId, new List<int> 
            {
                ServerConst.LOBBY_ID,
            });
            this.configLoader.loadMap(data, 1);
            this.configLoader.loadMap(data, 2);
            InitListenForClient(data);
            return data;
        }

        MonitorData CreateMonitorData(int serverId)
        {
            var data = new MonitorData();
            InitBaseData(data, serverId, new List<int> { ServerConst.LOC_ID });

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

        ServerData CreateServerData(int serverId)
        {
            if (serverId == ServerConst.LOC_ID)
            {
                return CreateLocData(serverId);
            }
            else if (serverId == ServerConst.AAA_ID)
            {
                return CreateAAAData(serverId);
            }
            else if (serverId == ServerConst.WEB_ID)
            {

            }
            else if (serverId == ServerConst.MONITOR_ID)
            {
                return CreateMonitorData(serverId);
            }
            else if (serverId == ServerConst.DB_ACCOUNT_ID ||
                serverId == ServerConst.DB_PLAYER_ID ||
                serverId == ServerConst.DB_LOG_ID)
            {
                return CreateDBData(serverId);
            }
            else if (serverId >= ServerConst.PM_START_ID && serverId <= ServerConst.PM_END_ID)
            {
                return CreatePMData(serverId);
            }
            else if (serverId == ServerConst.LOBBY_ID)
            {
                return CreateLobbyData(serverId);
            }
            else if (serverId >= ServerConst.BM_START_ID && serverId <= ServerConst.BM_END_ID)
            {
                return CreateBMData(serverId);
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
                serverId = ServerConst.LOC_ID,
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

            //// per-server data
            dataEntry.serverDatas = new Dictionary<int, ServerData>();
            foreach (var serverId in dataEntry.serverIds)
            {
                dataEntry.serverDatas.Add(serverId, this.CreateServerData(serverId));
            }
        }
    }
}