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

class _Loaders_
{
    public static JsonUtils JSON;

    public static T loadHomeJson<T>(string f)
    {
        string personalPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return JSON.parse<T>(File.ReadAllText(personalPath + "/config/" + f, Encoding.UTF8));
    }

    public static string loadConfigText(string f)
    {
        return File.ReadAllText("./config/" + f, Encoding.UTF8);
    }
    public static XmlElement parseConfigXml(string text)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        return doc.DocumentElement;
    }

    public static T loadPurposeConfigJson<T>(string f, Purpose purpose)
    {
        return JSON.parse<T>(File.ReadAllText("./Purposes/" + purpose + "/" + f, Encoding.UTF8));
    }

    public static string loadGameText(string f)
    {
        return File.ReadAllText("./gameConfig/" + f, Encoding.UTF8);
    }

    public static T loadGameJson<T>(string f)
    {
        return JSON.parse<T>(File.ReadAllText("./gameConfig/" + f, Encoding.UTF8));
    }
}

class _LocConfig_
{
    public string host;
}

class _VersionConfig_
{
    public string android;
    public string ios;
}

class _Helper_
{
    public int id;
    public Server s;
    public int port;
    public BaseRegister register;
}

public class ServerCreator
{
    static Dictionary<string, string> ParseArguments(string[] args)
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
    static SqlConfig initSqlConfig(string name, string purposeLowerCase)
    {
        return new SqlConfig
        {
            connectionLimit = 10,
            user = $"user_{purposeLowerCase}_{name}",
            password = $"gbits*{purposeLowerCase}*{name}*user*2020",
            database = $"{purposeLowerCase}_{name}",
        };
    }

    static ILoggerRepository log4netRepo;
    static void InitLog4net(List<string> loggerNamesToAdd)
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
    static void SetupLoggerForServer(Server server)
    {
        string stringId = Utils.numberId2stringId(server.baseData.id);
        server.logger = LogManager.GetLogger(log4netRepo.Name, stringId);
    }

    public static JsonUtils JSON = new JsonUtils();
    public static List<Server> Create(string[] args)
    {
        _Loaders_.JSON = JSON;

        //Console.WriteLine("Hello World!");
        var argMap = ParseArguments(args);
        List<int> serverIds = null;
        Purpose purpose = Purpose.Test;
        var processData = new ProcessData();

        string ids = argMap["ids"];
        if (ids == "all")
        {
            serverIds = new List<int> { 1, 2, 3, 11, 12, 13, 101 };
        }
        else
        {
            serverIds = JSON.parse<List<int>>(ids);
        }

        if (serverIds == null || serverIds.Count == 0)
        {
            Console.WriteLine("serverIds.Count == 0");
            return null;
        }

        purpose = Enum.Parse<Purpose>(argMap["purpose"]);

        ServerConst.initPorts(purpose);

        var thisMachineConfig = _Loaders_.loadHomeJson<ThisMachineConfig>("thisMachineConfig.json");
        var locConfig = _Loaders_.loadPurposeConfigJson<_LocConfig_>("locConfig.json", purpose);
        var versionConfig = _Loaders_.loadPurposeConfigJson<_VersionConfig_>("version.json", purpose);
        var purposeLowerCase = purpose.ToString().ToLower();

        var list = new List<_Helper_>();
        for (int i = 0; i < serverIds.Count; i++)
        {
            int id = serverIds[i];
            var h = new _Helper_();
            h.id = id;

            if (id == ServerConst.LOC_ID)
            {
                var s = h.s = new LocServer();

                s.locData = new LocData();

                s.locScript = new LocScript();
                s.locScript.server = s;

                h.port = ServerConst.LOC_PORT;
                h.register = new LocRegister();
            }
            else if (id == ServerConst.AAA_ID)
            {
                var s = h.s = new AAAServer();

                s.aaaData = new AAAData();

                s.aaaScript = new AAAScript();
                s.aaaScript.server = s;

                s.aaaSqlUtils = new AAASqlUtils();
                s.aaaSqlUtils.server = s;

                s.channelUuid = new AAAChannel_Uuid();
                s.channelUuid.server = s;

                s.channelDebug = new AAAChannel_Debug();
                s.channelDebug.server = s;

                s.channelApple = new AAAChannel_Apple();
                s.channelApple.server = s;

                s.channelIvy = new AAAChannel_Ivy();
                s.channelIvy.server = s;

                h.port = ServerConst.AAA_PORT;
                h.register = new AAARegister();
            }
            else if (id == ServerConst.WEB_ID)
            {
                continue;
            }
            else if (id == ServerConst.DB_ACCOUNT_ID)
            {
                var s = h.s = new DBAccountServer();

                s.dbData = new DBData();

                s.dbScript = new DBScript();
                s.dbScript.server = s;

                h.port = ServerConst.DB_ACCOUNT_PORT;
                h.register = new DBRegister();
                s.dbData.sqlConfig = initSqlConfig("account", purposeLowerCase);
            }
            else if (id == ServerConst.DB_PLAYER_ID)
            {
                var s = h.s = new DBPlayerServer();

                s.dbData = new DBData();

                s.dbScript = new DBScript();
                s.dbScript.server = s;

                h.port = ServerConst.DB_PLAYER_PORT;
                h.register = new DBRegister();
                s.dbData.sqlConfig = initSqlConfig("player", purposeLowerCase);
            }
            else if (id == ServerConst.DB_LOG_ID)
            {
                var s = h.s = new DBLogServer();

                s.dbData = new DBData();

                s.dbScript = new DBScript();
                s.dbScript.server = s;

                h.port = ServerConst.DB_LOG_PORT;
                h.register = new DBRegister();
                s.dbData.sqlConfig = initSqlConfig("log", purposeLowerCase);
            }
            else if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID)
            {
                var s = h.s = new PMServer();

                s.pmScript = new PMScript();
                s.pmScript.server = s;

                s.pmSqlUtils = new PMSqlUtils();
                s.pmSqlUtils.server = s;

                s.pmPlayerToSqlTablePlayer = new PMPlayerToSqlTablePlayer();
                s.pmPlayerToSqlTablePlayer.server = s;

                s.pmScriptCreateNewPlayer = new PMScriptCreateNewPlayer();
                s.pmScriptCreateNewPlayer.server = s;

                var pmData = s.pmData = new PMData();
                if (purpose == Purpose.Test)
                {
                    pmData.allowNewPlayer = true;
                }
                s.gameScript = new GameScriptServer();
                s.gameScript.init(pmData, s);
                h.port = ServerConst.PM_START_PORT + (id - ServerConst.PM_START_ID);
                h.register = new PMRegister();
            }
            else
            {
                continue;
            }
            list.Add(h);
        }


        var locloc = new Loc();
        locloc.id = ServerConst.LOC_ID;
        locloc.inIp = locConfig.host;
        locloc.outIp = null;
        locloc.outDomain = null;
        locloc.port = ServerConst.LOC_PORT;

        var timezoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;

        var allServers = new List<Server>();
        List<string> loggerNamesToAdd = new List<string>();
        for (int i = 0; i < list.Count; i++)
        {
            var h = list[i];
            //addLogConfig(Utils.numberId2stringId(h.id));
            //addLogConfig('error-' + Utils.numberId2stringId(h.id));
            allServers.Add(h.s);
            loggerNamesToAdd.Add(Utils.numberId2stringId(h.id));
        }

        InitLog4net(loggerNamesToAdd);

        // 公共字段
        for (int i = 0; i < list.Count; i++)
        {
            var h = list[i];
            var s = h.s;

            s.purpose = purpose;
            s.androidVersion = versionConfig.android;
            s.iOSVersion = versionConfig.ios;

            s.baseData = new BaseData();
            s.baseData.timezoneOffset = timezoneOffset;
            s.baseData.processData = processData;
            s.baseData.id = h.id;

            s.baseScript = new BaseScript();
            s.baseScript.server = s;

            s.serverNetwork = new NetProtoTcp();
            ((NetProtoTcp)s.serverNetwork).server = s;

            s.timerScript = new TimerScript();
            s.timerScript.server = s;

            SetupLoggerForServer(s);

            // s.netProto = new NetworkProtocolWS();
            // (s.netProto as NetworkProtocolWS).server = s;

            s.dispatcher = new MessageDispatcher();
            s.dispatcher.server = s;

            s.utils = new Utils();
            s.scUtils = new SCUtils();
            s.JSON = new JsonUtils();

            s.sqlLog = new SqlLog();
            s.sqlLog.server = s;

            // s.payLtSqlUtils = new PayLtSqlUtils();
            // s.payLtSqlUtils.server = s;

            // s.payIvySqlUtils = new PayIvySqlUtils();
            // s.payIvySqlUtils.server = s;

            // s.logger = log4js.getLogger(Utils.numberId2stringId(h.id));
            // s.errorLogger = log4js.getLogger('error-' + Utils.numberId2stringId(h.id));

            // init data.allServers
            s.baseData.allServers = allServers;

            // init known locs
            s.baseScript.addKnownLoc(locloc);

            var selfLoc = new Loc
            {
                id = s.baseData.id,
                inIp = thisMachineConfig.inIp,
                outIp = thisMachineConfig.outIp,
                outDomain = thisMachineConfig.outDomain,
                port = h.port,
            };
            s.baseScript.addKnownLoc(selfLoc);
            h.register.register(s);
        }

        var ret = new List<Server>();
        for (int i = 0; i < list.Count; i++)
        {
            ret.Add(list[i].s);
        }
        return ret;
    }
}