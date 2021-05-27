
public class Utils
{
    static public string numberId2stringId(int id)
    {
        if (id == ServerConst.LOC_ID) {
            return "loc";
        }
        if (id == ServerConst.AAA_ID) {
            return "aaa";
        }
        if (id == ServerConst.WEB_ID) {
            return "web";
        }
        if (id == ServerConst.DB_ACCOUNT_ID) {
            return "dbAccount";
        }
        if (id == ServerConst.DB_PLAYER_ID) {
            return "dbPlayer";
        }
        if (id == ServerConst.DB_LOG_ID) {
            return "dbLog";
        }
        if (id == ServerConst.MONITOR_ID) {
            return "monitor";
        }
        if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID) {
            return "pm" + id;
        }
        return null;
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
                    return int.Parse(id.Substring("pm".Length));
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