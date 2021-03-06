using Data;
namespace Script
{
    public class Utils
    {
        static public string numberId2stringId(int serverId)
        {
            if (serverId == ServerConst.LOC_ID)
            {
                return "loc";
            }
            if (serverId == ServerConst.AAA_ID)
            {
                return "aaa";
            }
            if (serverId == ServerConst.WEB_ID)
            {
                return "web";
            }
            if (serverId == ServerConst.DB_ACCOUNT_ID)
            {
                return "dbAccount";
            }
            if (serverId == ServerConst.DB_PLAYER_ID)
            {
                return "dbPlayer";
            }
            if (serverId == ServerConst.DB_LOG_ID)
            {
                return "dbLog";
            }
            if (serverId == ServerConst.MONITOR_ID)
            {
                return "monitor";
            }
            if (serverId == ServerConst.LOBBY_ID)
            {
                return "lobby";
            }
            if (serverId >= ServerConst.PM_START_ID && serverId <= ServerConst.PM_END_ID)
            {
                return "pm" + serverId;
            }
            if (serverId >= ServerConst.BM_START_ID && serverId <= ServerConst.BM_END_ID)
            {
                return "bm" + serverId;
            }
            return null;
        }
        public int stringId2numberId(string serverId)
        {
            switch (serverId)
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

                case "lobby":
                    return ServerConst.LOBBY_ID;

                default:
                    {
                        if (serverId.StartsWith("pm"))
                            return int.Parse(serverId.Substring("pm".Length));
                        else if (serverId.StartsWith("bm"))
                            return int.Parse(serverId.Substring("bm".Length));
                        else
                            throw new System.Exception();
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
}