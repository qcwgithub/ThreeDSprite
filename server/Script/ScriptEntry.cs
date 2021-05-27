using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Data;
using log4net;
using log4net.Config;
using log4net.Repository;
using System.Linq;

namespace Script
{
    public class ScriptEntry
    {
        Dictionary<string, string> ParseArguments(string[] args)
        {
            var argMap = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                int index = arg.IndexOf('=');
                if (index > 0)
                {
                    string key = arg.Substring(0, index);
                    string value = arg.Substring(index + 1);
                    Console.WriteLine(key + ": " + value);
                    argMap.Add(key, value);
                }
            }
            return argMap;
        }

        ILoggerRepository log4netRepo;
        void InitLog4net(List<string> loggerNamesToAdd)
        {
            string xmlText = _Loaders_.loadConfigText("log4netConfig.xml");
            XmlElement xmlRoot = _Loaders_.parseConfigXml(xmlText);

            // find templates
            XmlNode fileAppenderTemplate = null;
            XmlNode loggerTemplate = null;

            XmlNode node = xmlRoot.FirstChild;
            while (node != null)
            {
                if (fileAppenderTemplate == null && node.Name == "appender" && node.Attributes["name"].Value == "file")
                {
                    fileAppenderTemplate = node;
                }
                if (loggerTemplate == null && node.Name == "logger")
                {
                    loggerTemplate = node;
                }
                node = node.NextSibling;
            }

            if (fileAppenderTemplate == null || loggerTemplate == null)
            {
                throw new Exception("init log4net failed 1");
            }

            // 
            foreach (string loggerName in loggerNamesToAdd)
            {
                // 1
                XmlNode fileAppender = fileAppenderTemplate.Clone();
                fileAppenderTemplate.ParentNode.AppendChild(fileAppender);

                fileAppender.Attributes["name"].Value = "file_" + loggerName;
                bool success = false;
                node = fileAppender.FirstChild;
                while (node != null)
                {
                    if (node.Name == "file")
                    {
                        success = true;
                        node.Attributes["value"].Value = "./logs/" + loggerName;
                        break;
                    }
                    node = node.NextSibling;
                }
                if (!success)
                {
                    throw new Exception("init log4net failed 2");
                }

                // 2
                XmlNode logger = loggerTemplate.Clone();
                loggerTemplate.ParentNode.AppendChild(logger);

                logger.Attributes["name"].Value = loggerName;
                success = false;
                node = logger.FirstChild;
                while (node != null)
                {
                    if (node.Name == "appender-ref" && node.Attributes["ref"].Value == "file")
                    {
                        success = true;
                        node.Attributes["ref"].Value = "file_" + loggerName;
                        break;
                    }
                    node = node.NextSibling;
                }
                if (!success)
                {
                    throw new Exception("init log4net failed 3");
                }
            }

            log4netRepo = LogManager.CreateRepository("my_log4net_repo");
            XmlConfigurator.Configure(log4netRepo, xmlRoot);
        }

        void createData(int id, ServerData data)
        {
            if (id == ServerConst.LOC_ID)
            {
                data.locData = new LocData();
            }
            else if (id == ServerConst.AAA_ID)
            {
                data.aaaData = new AAAData();
            }
            else if (id == ServerConst.WEB_ID)
            {

            }
            else if (id == ServerConst.DB_ACCOUNT_ID)
            {
                data.dbData = new DBData();
            }
            else if (id == ServerConst.DB_PLAYER_ID)
            {
                data.dbData = new DBData();
            }
            else if (id == ServerConst.DB_LOG_ID)
            {
                data.dbData = new DBData();
            }
            else if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID)
            {
                data.pmData = new PMData();
            }
        }

        private bool InitDataOnce(string[] args, GlobalData data)
        {
            data.processData = new ProcessData();
            var timezoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;

            var argMap = ParseArguments(args);
            string ids = argMap["ids"];
            if (ids == "all")
                data.serverIds = new List<int> { 1, 2, 3, 11, 12, 13, 101 };
            else
                data.serverIds = JSON.parse<List<int>>(ids);

            if (data.serverIds == null || data.serverIds.Count == 0)
            {
                Console.WriteLine("serverIds.Count == 0");
                return false;
            }

            data.purpose = Enum.Parse<Purpose>(argMap["purpose"]);
            data.timezoneOffset = timezoneOffset;
            ServerConst.initPorts(data.purpose);

            var versionConfig = _Loaders_.loadPurposeConfigJson<_VersionConfig_>("version.json", data.purpose);
            data.androidVersion = versionConfig.android;
            data.iOSVersion = versionConfig.ios;

            //// per-server data

            var thisMachineConfig = _Loaders_.loadHomeJson<ThisMachineConfig>("thisMachineConfig.json");
            var locConfig = _Loaders_.loadPurposeConfigJson<_LocConfig_>("locConfig.json", data.purpose);

            var locloc = new Loc();
            locloc.id = ServerConst.LOC_ID;
            locloc.inIp = locConfig.host;
            locloc.outIp = null;
            locloc.outDomain = null;
            locloc.port = ServerConst.LOC_PORT;

            this.InitLog4net(data.serverIds.Select(_ => Utils.numberId2stringId(_)).ToList());

            data.serverDatas = new Dictionary<int, ServerData>();
            foreach (var serverId in data.serverIds)
            {
                ServerData serverData = new ServerData();

                // baseData
                var baseData = serverData.baseData = new BaseData();
                baseData.id = serverId;

                baseData.knownLocs[locloc.id] = locloc;

                var selfLoc = new Loc
                {
                    id = serverId,
                    inIp = thisMachineConfig.inIp,
                    outIp = thisMachineConfig.outIp,
                    outDomain = thisMachineConfig.outDomain,
                    port = ServerConst.getPortByServerId(serverId),
                };
                baseData.knownLocs[selfLoc.id] = selfLoc;

                baseData.logger = LogManager.GetLogger(this.log4netRepo.Name, Utils.numberId2stringId(serverId));
                this.createData(serverId, serverData);
                data.serverDatas.Add(serverId, serverData);
            }
            return true;
        }

        public JsonUtils JSON = new JsonUtils();
        public void OnLoad(string[] args, GlobalData data)
        {
            if (!data.inited)
            {
                data.inited = true;

                this.InitDataOnce(args, data);

                //------------------------
                // 异步方法全部会回掉到主线程
                SynchronizationContext.SetSynchronizationContext(ET.ThreadSynchronizationContext.Instance);

                List<Server> servers = ServerCreator.Create(data, JSON);
                foreach (Server server in servers)
                {
                    server.dispatcher.dispatch(null, MsgType.Start, null, null);
                }
            }

            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    ET.ThreadSynchronizationContext.Instance.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
