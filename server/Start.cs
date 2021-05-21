using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class _Loaders_
{
    public static T loadHomeJson<T>(string f)
    {
        return Program.jsonUtils.parse<T>(File.ReadAllText(Environment.SpecialFolder.Personal + "/config/" + f, Encoding.UTF8));
    }

    public static T loadConfigJson<T>(string f, Purpose purpose)
    {
        return Program.jsonUtils.parse<T>(File.ReadAllText("./Purposes/" + purpose + "/" + f, Encoding.UTF8));
    }

    public static string loadGameText(string f)
    {
        return File.ReadAllText("./gameConfig/" + f, Encoding.UTF8);
    }

    public static T loadGameJson<T>(string f)
    {
        return Program.jsonUtils.parse<T>(File.ReadAllText("./gameConfig/" + f, Encoding.UTF8));
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

class Program
{
    public static JsonUtils jsonUtils = new JsonUtils();
    static Dictionary<string, string> ParseArguments(string[] args)
    {
        var argMap = new Dictionary<string, string>();
        for (int i = 1; i < args.Length; i++)
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
    static void Main(string[] args)
    {
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
            serverIds = jsonUtils.parse<List<int>>(ids);
        }

        if (serverIds == null || serverIds.Count == 0)
        {
            Console.WriteLine("serverIds.Count == 0");
            return;
        }

        purpose = Enum.Parse<Purpose>(argMap["purpose"]);

        ServerConst.initPorts(purpose);


        var thisMachineConfig = _Loaders_.loadHomeJson<ThisMachineConfig>("thisMachineConfig.json");
        var locConfig = _Loaders_.loadConfigJson<_LocConfig_>("locConfig.json", purpose);
        var versionConfig = _Loaders_.loadConfigJson<_VersionConfig_>("version.json", purpose);
        var purposeLowerCase = purpose.ToString().ToLower();

        var list = new List<_Helper_>();
        for (int i = 0; i < serverIds.Count; i++)
        {
            int id = serverIds[i];
            var h = new _Helper_();
            h.id = id;
            list.Add(h);

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

                // s.channelLeiting = null;//new AAAChannel_Leiting();
                // s.channelLeiting.server = s;

                s.channelIvy = new AAAChannel_Ivy();
                s.channelIvy.server = s;

                h.port = ServerConst.AAA_PORT;
                h.register = new AAARegister();
            }
            else if (id == ServerConst.WEB_ID)
            {

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

                // s.probability = new Probability().init(pmData, s);
                // s.turnTableScript = new TurnTableScript().init(pmData, s);
                // s.shoppingMallScript = new ShoppingMallScript().init(pmData, s);
                // s.dailySigninScript = new DailySigninScript().init(pmData, s);
                // s.adDiamondScript = new ADDiamondScript().init(pmData, s);
                // s.loginRewardScript = new LoginRewardScript().init(pmData, s);
                // s.shipScript = new ShipScript().init(pmData, s);
                // s.vipScript = new VipScript().init(pmData, s);
                // s.commonScript = new CommonScript().init(pmData, s);

                // s.dailyTaskScript = new DailyTaskScript().init(pmData, s);
                // s.townTaskScript = new TownTaskScript().init(pmData, s);
                // s.doctorKTaskScript = new DoctorKTaskScript().init(pmData, s);
                // s.storyTaskScript = new StoryTaskScript().init(pmData, s);
                // s.townScript = new TownScript().init(pmData, s);

                // s.freeBoxScript = new FreeBoxScript().init(pmData, s);
                // s.buildingScript = new BuildingScript().init(pmData, s);
                // s.bankScript = new BankScript().init(pmData, s);
                // s.offlineScript = new OfflineScript().init(pmData, s);
                s.gameScript = new GameScriptServer();
                s.gameScript.init(pmData, s);
                // s.partyScript = new PartyScript().init(pmData, s);
                // s.ksScript = new KSScript().init(pmData, s);
                // s.actLevelAwardScript = new ActivityLevelAwardScript().init(pmData, s);
                // s.actScript = new ActivityScript().init(pmData, s);

                ////
                // pmData.activityConfig = ConfigScript.processActivityConfig(Loaders.loadGameJson('activity.json'));
                // pmData.activityLevelAwardConfig = ConfigScript.processActivityLevelAwardConfig(Loaders.loadGameJson('activity_level_award.json'));
                // pmData.bankConfig = Loaders.loadGameJson('bank.json');
                // ConfigScript.processBankConfig(pmData.bankConfig);

                // pmData.commonConfig = Loaders.loadGameJson('common.json');
                // pmData.adConfig = Loaders.loadGameJson('ad.json');
                // pmData.bonusConfig = Loaders.loadGameJson('bonus.json');
                // ConfigScript.processBonusCfg(pmData.bonusConfig);

                // pmData.turnTableConfig = Loaders.loadGameJson('casino.json') as CasinoConfig;
                // ConfigScript.processCasinoConfig(pmData.turnTableConfig);

                // pmData.consumerConfig = Loaders.loadGameJson('consumer.json');
                // pmData.defaultProfileConfig = Loaders.loadGameText('defaultProfile.json');
                // pmData.houseConfigs = ConfigScript.processHouseConfig(Loaders.loadGameText('houses.csv'));
                // pmData.housePriceConfigs = ConfigScript.processHousePriceConfig(Loaders.loadGameText('house_price.csv'));
                // pmData.iapConfig = ConfigScript.processIapConfig(Loaders.loadGameJson('iap.json'));

                // pmData.npcsConfig.npcConfigs = CsvUtils.parseToObjectArray(Loaders.loadGameText('npc.csv'));
                // ConfigScript.processNpcsConfig(pmData.npcsConfig);

                // pmData.partyConfig = ConfigScript.processPartyConfig(Loaders.loadGameJson('party.json'), Loaders.loadGameText('party.csv'));
                // pmData.profileConfig = ConfigScript.processProfileConfig(Loaders.loadGameJson('profile.json'));
                // pmData.rankConfig = Loaders.loadGameJson('rank.json');
                // pmData.rewardConfig = Loaders.loadGameJson('reward.json');
                // pmData.shipConfig = ConfigScript.processShipConfig(Loaders.loadGameJson('ship.json') as ShipConfig);
                // pmData.shopConfig = Loaders.loadGameJson('shop.json');

                // pmData.townTaskConfig = new TownTaskConfig();
                // pmData.townTaskConfig.items = CsvUtils.parseToObjectArray(Loaders.loadGameText('task_town.csv'));
                // ConfigScript.processTownTaskConfig(pmData.townTaskConfig);

                // pmData.dailyTaskConfig = Loaders.loadGameJson('task_daily.json');
                // ConfigScript.processDailyTaskConfig(pmData.dailyTaskConfig);

                // pmData.doctorKTaskConfig = Loaders.loadGameJson('task_doctork.json');
                // ConfigScript.processDoctorKTaskConfig(pmData.doctorKTaskConfig);

                // pmData.storyTaskConfig = Loaders.loadGameJson('task_story.json');
                // ConfigScript.processStoryTaskConfig(pmData.storyTaskConfig);

                // pmData.townConfig = Loaders.loadGameJson('town.json');
                // ConfigScript.processTownConfig(pmData.townConfig);

                // pmData.ksConfig = Loaders.loadGameJson('kingstreet.json');

                // pmData.buffConfigMap = ConfigScript.processBuffConfig(Loaders.loadGameJson('buffs.json'));

                // pmData.landmarkConfigMap = ConfigScript.processLandmarkConfig(
                //     Loaders.loadGameText('landmark_ShoppingMall.csv'),
                //     Loaders.loadGameText('landmark_Casino.csv'),
                //     Loaders.loadGameText('landmark_DoctorK.csv'));

                h.port = ServerConst.PM_START_PORT + (id - ServerConst.PM_START_ID);
                h.register = new PMRegister();
            }
        }


        var locloc = new Loc();
        locloc.id = ServerConst.LOC_ID;
        locloc.inIp = locConfig.host;
        locloc.outIp = null;
        locloc.outDomain = null;
        locloc.port = ServerConst.LOC_PORT;

        var timezoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;

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

            s.purpose = purpose;
            s.androidVersion = versionConfig.android;
            s.iOSVersion = versionConfig.ios;

            s.baseData = new BaseData();
            s.baseData.timezoneOffset = timezoneOffset;
            s.baseData.processData = processData;
            s.baseData.id = h.id;

            s.baseScript = new BaseScript();
            s.baseScript.server = s;

            // s.netProto = new NetworkProtocolWS();
            // (s.netProto as NetworkProtocolWS).server = s;

            s.dispatcher = new MessageDispatcher();
            s.dispatcher.server = s;

            s.coroutineMgr = new CoroutineManager();
            s.coroutineMgr.server = s;

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

        for (int i = 0; i < list.Count; i++)
        {
            var h = list[i];
            h.s.dispatcher.dispatch(null, MsgType.Start, new object(), null);
        }
    }
}