using System;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class DataCreation
    {
        void InitBaseData(ServerBaseData baseData, int serverId)
        {
            baseData.id = serverId;

            baseData.tcpListener = new TcpListenerData() { serverData = baseData };
            baseData.knownLocs[this.dataEntry.locLoc.id] = dataEntry.locLoc;

            var selfLoc = new Loc
            {
                id = serverId,
                inIp = this.dataEntry.thisMachineConfig.inIp,
                outIp = this.dataEntry.thisMachineConfig.outIp,
                outDomain = this.dataEntry.thisMachineConfig.outDomain,
                port = ServerConst.getPortByServerId(serverId),
            };
            baseData.knownLocs[selfLoc.id] = selfLoc;

            baseData.logger = this.log4NetCreation.getLogger(Utils.numberId2stringId(serverId));
        }

        LocData CreateLocData(int id)
        {
            var data = new LocData();
            InitBaseData(data, id);
            return data;
        }

        AAAData CreateAAAData(int id)
        {
            var data = new AAAData();
            InitBaseData(data, id);
            return data;
        }

        DBData CreateDBData(int id)
        {
            var data = new DBData();
            InitBaseData(data, id);
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

            return data;
        }

        PMData CreatePMData(int id)
        {
            var data = new PMData();
            InitBaseData(data, id);
            return data;
        }

        ServerBaseData CreateData(int id)
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

            //// per-server data

            dataEntry.thisMachineConfig = configLoader.ThisMachineConfig;
            dataEntry.locLoc = new Loc()
            {
                id = ServerConst.LOC_ID,
                inIp = configLoader.LocConfig.host,
                outIp = null,
                outDomain = null,
                port = ServerConst.LOC_PORT,
            };

            dataEntry.serverDatas = new Dictionary<int, ServerBaseData>();
            foreach (var serverId in dataEntry.serverIds)
            {
                dataEntry.serverDatas.Add(serverId, this.CreateData(serverId));
            }
        }
    }
}