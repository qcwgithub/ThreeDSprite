public class JSON
{
    public static string stringify(object obj)
    {
        return null;
    }
}

public class FakeLogger
{
    public void debug(params object[] args)
    {

    }
    public void info(params object[] args)
    {

    }
    public void warn(params object[] args)
    {
        
    }
    public void error(params object[] args)
    {
        
    }
}


// Server 提供给 IScript 数据、其他脚本的访问
public class Server {
    public Purpose purpose;
    public string androidVersion = null;
    public string iOSVersion = null;

    //// common ------------------------------
    public BaseData baseData = null;
    public BaseScript baseScript = null;
    public INetworkProtocol netProto = null;

    public MessageDispatcher dispatcher = null;
    public CoroutineManager coroutineMgr = null;
    public FakeLogger logger = null;
    public FakeLogger errorLogger = null;
    public SqlLog sqlLog = null;
    public Utils utils = null;
    public SCUtils scUtils = null;

    //// aaaa and pm
    public PayLtSqlUtils payLtSqlUtils = null;
    public PayIvySqlUtils payIvySqlUtils = null;

    //// aaa ------------------------------
    public AAAData aaaData = null;
    public AAAScript aaaScript = null;
    public AAASqlUtils aaaSqlUtils = null;
    public AAAChannel_Uuid channelUuid = null;
    public AAAChannel_Debug channelDebug = null;
    public AAAChannel_Apple channelApple = null;
    public AAAChannel_Leiting channelLeiting = null;
    public AAAChannel_Ivy channelIvy = null;

    //// db ------------------------------
    public DBData dbData = null;
    public DBScript dbScript = null;

    //// monitor ------------------------------
    public MonitorData monitorData = null;
    public MonitorScript monitorScript = null;

    //// web ------------------------------
    public WebData webData = null;
    public WebScript webScript = null;

    //// loc ------------------------------
    public LocData locData = null;
    public LocScript locScript = null;

    //// pm ------------------------------
    public PMData pmData = null;
    public PMScript pmScript = null;
    public PMSqlUtils pmSqlUtils = null;
    // ProfileEnsures profileEnsures = null;
    public PMPlayerToSqlTablePlayer pmPlayerToSqlTablePlayer = null;

    public PMScriptCreateNewPlayer pmScriptCreateNewPlayer = null;
    public GameScript gameScript = null;
}