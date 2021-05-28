using System;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class DataCreation
    {
        void InitBaseData(BaseData baseData, int serverId)
        {
            baseData.id = serverId;

            baseData.knownLocs[this.global.locLoc.id] = global.locLoc;

            var selfLoc = new Loc
            {
                id = serverId,
                inIp = this.global.thisMachineConfig.inIp,
                outIp = this.global.thisMachineConfig.outIp,
                outDomain = this.global.thisMachineConfig.outDomain,
                port = ServerConst.getPortByServerId(serverId),
            };
            baseData.knownLocs[selfLoc.id] = selfLoc;

            baseData.logger = this.log4NetCreation.getLogger(Utils.numberId2stringId(serverId));
        }

        BaseData CreateData(int id)
        {
            BaseData data = null;
            if (id == ServerConst.LOC_ID)
            {
                data = new LocData();
                this.InitBaseData(data, id);
            }
            else if (id == ServerConst.AAA_ID)
            {
                data = new AAAData();
                this.InitBaseData(data, id);
            }
            else if (id == ServerConst.WEB_ID)
            {

            }
            else if (id == ServerConst.DB_ACCOUNT_ID)
            {
                var dbData = new DBData();
                data = dbData;
                this.InitBaseData(data, id);
                dbData.sqlConfig = this.configLoader.AccountSqlConfig;
            }
            else if (id == ServerConst.DB_PLAYER_ID)
            {
                var dbData = new DBData();
                data = dbData;
                this.InitBaseData(data, id);
                dbData.sqlConfig = this.configLoader.PlayerSqlConfig;
            }
            else if (id == ServerConst.DB_LOG_ID)
            {
                var dbData = new DBData();
                data = dbData;
                this.InitBaseData(data, id);
                dbData.sqlConfig = this.configLoader.LogSqlConfig;
            }
            else if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID)
            {
                data = new PMData();
                this.InitBaseData(data, id);
            }

            return data;
        }

        GlobalData global;
        ConfigLoader configLoader;
        Log4netCreation log4NetCreation;
        public void Create(GlobalData global, ConfigLoader configLoader, List<int> serverIds, Purpose purpose, Log4netCreation log4NetCreation)
        {
            this.global = global;
            this.configLoader = configLoader;
            this.log4NetCreation = log4NetCreation;

            global.processData = new ProcessData();
            global.serverIds = serverIds;
            global.purpose = purpose;
            var timezoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;

            global.timezoneOffset = timezoneOffset;
            ServerConst.initPorts(global.purpose);

            var versionConfig = configLoader.VersionConfig;
            global.androidVersion = versionConfig.android;
            global.iOSVersion = versionConfig.ios;

            //// per-server data

            global.thisMachineConfig = configLoader.ThisMachineConfig;
            global.locLoc = new Loc()
            {
                id = ServerConst.LOC_ID,
                inIp = configLoader.LocConfig.host,
                outIp = null,
                outDomain = null,
                port = ServerConst.LOC_PORT,
            };

            global.serverDatas = new Dictionary<int, BaseData>();
            foreach (var serverId in global.serverIds)
            {
                var data = this.CreateData(serverId);
                global.serverDatas.Add(serverId, data);
            }
        }
    }
}