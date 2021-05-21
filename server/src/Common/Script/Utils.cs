
public class Utils
{
    static string getTrace(/*object caller*/)
    {
        // var original = Error.prepareStackTrace;
        var object error = { };
        // Error.prepareStackTrace = prepareStackTrace;
        Error.captureStackTrace(error, null/*caller || Utils.getTrace*/);
        var stack = error.stack;
        // Error.prepareStackTrace = original;
        return stack;
    }

    static public string numberId2stringId(int id)
    {
        switch (id)
        {
            case ServerConst.LOC_ID:
                return "loc";

            case ServerConst.AAA_ID:
                return "aaa";

            case ServerConst.WEB_ID:
                return "web";

            case ServerConst.DB_ACCOUNT_ID:
                return "dbAccount";

            case ServerConst.DB_PLAYER_ID:
                return "dbPlayer";

            case ServerConst.DB_LOG_ID:
                return "dbLog";

            case ServerConst.MONITOR_ID:
                return "monitor";

            default:
                {
                    if (id < ServerConst.PM_START_ID || id > ServerConst.PM_END_ID)
                    {
                        return null;
                    }
                    return "pm" + id;
                }
        }
    }
    public int stringId2numberId(string id)
    {
        switch (id)
        {
            case "loc":
                return ServerConst.LOC_ID;

            case "aaa":
                return ServerConst.AAA_ID;

            case "web":
                return ServerConst.WEB_ID;

            case "dbAccount":
                return ServerConst.DB_ACCOUNT_ID;

            case "dbPlayer":
                return ServerConst.DB_PLAYER_ID;

            case "dbLog":
                return ServerConst.DB_LOG_ID;

            case "monitor":
                return ServerConst.MONITOR_ID;

            default:
                {
                    return parseInt(id.substring("pm".length));
                }
        }
    }

    static bool isValidPurpose(Purpose purpose)
    {
        for (int i = 0; i < (int)Purpose.Count; i++)
        {
            if ((int)purpose == i)
            {
                return true;
            }
        }
        return false;
    }
}