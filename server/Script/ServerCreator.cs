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


        public static List<Server> Create(Data.GlobalData data, JsonUtils JSON)
        {
            _Loaders_.JSON = JSON;

            
            var versionConfig = _Loaders_.loadPurposeConfigJson<_VersionConfig_>("version.json", data.purpose);
            var purposeLowerCase = data.purpose.ToString().ToLower();

            var list = new List<_Helper_>();
            for (int i = 0; i < data.serverIds.Count; i++)
            {
                int id = data.serverIds[i];
                var h = new _Helper_();
                h.id = id;

                if (id == ServerConst.LOC_ID)
                {
                    var s = h.s = new Server();

                    s.locData = new LocData();

                    s.locScript = new LocScript();
                    s.locScript.server = s;

                    h.register = new LocRegister();
                }
                else if (id == ServerConst.AAA_ID)
                {
                    var s = h.s = new Server();

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

                    h.register = new AAARegister();
                }
                else if (id == ServerConst.WEB_ID)
                {
                    continue;
                }
                else if (id == ServerConst.DB_ACCOUNT_ID)
                {
                    var s = h.s = new Server();

                    s.dbData = new DBData();

                    s.dbScript = new DBScript();
                    s.dbScript.server = s;

                    h.register = new DBRegister();
                    s.dbData.sqlConfig = initSqlConfig("account", purposeLowerCase);
                }
                else if (id == ServerConst.DB_PLAYER_ID)
                {
                    var s = h.s = new Server();

                    s.dbData = new DBData();

                    s.dbScript = new DBScript();
                    s.dbScript.server = s;

                    h.register = new DBRegister();
                    s.dbData.sqlConfig = initSqlConfig("player", purposeLowerCase);
                }
                else if (id == ServerConst.DB_LOG_ID)
                {
                    var s = h.s = new Server();

                    s.dbData = new DBData();

                    s.dbScript = new DBScript();
                    s.dbScript.server = s;

                    h.register = new DBRegister();
                    s.dbData.sqlConfig = initSqlConfig("log", purposeLowerCase);
                }
                else if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID)
                {
                    var s = h.s = new Server();

                    s.pmScript = new PMScript();
                    s.pmScript.server = s;

                    s.pmSqlUtils = new PMSqlUtils();
                    s.pmSqlUtils.server = s;

                    s.pmPlayerToSqlTablePlayer = new PMPlayerToSqlTablePlayer();
                    s.pmPlayerToSqlTablePlayer.server = s;

                    s.pmScriptCreateNewPlayer = new PMScriptCreateNewPlayer();
                    s.pmScriptCreateNewPlayer.server = s;

                    var pmData = s.pmData = new PMData();
                    s.gameScript = new GameScriptServer();
                    s.gameScript.init(pmData, s);
                    h.register = new PMRegister();
                }
                else
                {
                    continue;
                }
                list.Add(h);
            }

            var allServers = new List<Server>();
            for (int i = 0; i < list.Count; i++)
            {
                var h = list[i];
                //addLogConfig(Utils.numberId2stringId(h.id));
                //addLogConfig('error-' + Utils.numberId2stringId(h.id));
                allServers.Add(h.s);
            }

            // 公共字段
            for (int i = 0; i < list.Count; i++)
            {
                var h = list[i];
                var s = h.s;

                s.purpose = data.purpose;
                s.androidVersion = versionConfig.android;
                s.iOSVersion = versionConfig.ios;

                s.baseData = new BaseData();
                s.baseData.id = h.id;

                s.baseScript = new BaseScript();
                s.baseScript.server = s;

                s.serverNetwork = new NetProtoTcp();
                ((NetProtoTcp)s.serverNetwork).server = s;

                s.timerScript = new TimerScript();
                s.timerScript.server = s;

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
}