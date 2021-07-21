using System;

public class ServerConst
{
    public static string CLIENT_SIGN = "kk66iioo773452m";
    public static string SERVER_SIGN = "fsd4534fh3hssfd";

    public static int LOC_ID = 1;
    public static int LOC_PORT = 0;

    public static int AAA_ID = 2;
    public static int AAA_IN_PORT = 0; // 对外端口8000-9000
    public static int AAA_OUT_PORT = 0; // 对外端口8000-9000
    public static int AAA_LT_NOTIFY_PORT = 0;
    public static int AAA_IVY_NOTIFY_PORT = 0;

    public static int WEB_ID = 3;
    public static int WEB_PORT = 0; // 对外端口8000-9000

    public static int MONITOR_ID = 4;
    public static int MONITOR_PORT = 0;

    public static int LOBBY_ID = 5;
    public static int LOBBY_IN_PORT = 0;

    public static int DB_ACCOUNT_ID = 11;
    public static int DB_ACCOUNT_PORT = 0;

    public static int DB_PLAYER_ID = 12;
    public static int DB_PLAYER_PORT = 0;

    public static int DB_LOG_ID = 13;
    public static int DB_LOG_PORT = 0;

    public static int PM_START_ID = 101;
    public static int PM_END_ID = 199;
    // public static int PM_START_IN_PORT = 0;
    public static int PM_START_OUT_PORT = 0;

    public static int BM_START_ID = 201;
    public static int BM_END_ID = 399;
    //public static int BM_START_IN_PORT = 0;
    public static int BM_START_OUT_PORT = 0;

    // 域名
    // cn https://hecxxzlogin.jysyx.net/
    // global https://hecxxzloginglobal.jysyx.net/

    // 服务器
    // SH1 81.69.173.127
    // SH2 81.69.202.219
    // HK1 124.156.153.109

    public static bool inited = false;
    public static void initPorts(Purpose purpose)
    {
        inited = true;
        if (purpose == Purpose.tds_test)
        {
            // [3001, 3009]
            // [8001, 8009]
            LOC_PORT = 3001;
            MONITOR_PORT = 3002;
            DB_ACCOUNT_PORT = 3003;
            DB_PLAYER_PORT = 3004;
            DB_LOG_PORT = 3005;
            AAA_IN_PORT = 3006;
            LOBBY_IN_PORT = 3007;

            AAA_OUT_PORT = 8001; // cn -> SH1
            WEB_PORT = 8002; // cn -> SH1
            AAA_LT_NOTIFY_PORT = 8004;

            // PM_START_IN_PORT = 3007;

            PM_START_OUT_PORT = 8005; // cn -> SH1
            AAA_IVY_NOTIFY_PORT = 8006;
            
            // BM_START_IN_PORT = 0;
            BM_START_OUT_PORT = 8009;
        }
        // else if (purpose == Purpose.Review) {
        //     // [3011, 3019]
        //     // [8011, 8019]
        //     this.LOC_PORT = 3011;
        //     this.MONITOR_PORT = 3012;
        //     this.DB_ACCOUNT_PORT = 3013;
        //     this.DB_PLAYER_PORT = 3014;
        //     this.DB_LOG_PORT = 3015;
        //     this.AAA_PORT = 8011; // cn -> SH1
        //     this.WEB_PORT = 8012; // cn -> SH1
        //     this.AAA_LT_NOTIFY_PORT = 8014;
        //     this.PM_START_PORT = 8015;
        //     this.AAA_IVY_NOTIFY_PORT = 8016;
        // }
        else if (purpose == Purpose.tds_ios)
        {
            // [3021, 3050] 如被占用则换一个
            // [8021, 8100] 如被占用：非PM则换一个，PM则不使用那个ID的PM
            LOC_PORT = 3021;
            MONITOR_PORT = 3022;
            DB_ACCOUNT_PORT = 3023;
            DB_PLAYER_PORT = 3024;
            DB_LOG_PORT = 3025;
            AAA_IN_PORT = 3026;

            AAA_OUT_PORT = 8021; // cn -> SH1
            WEB_PORT = 8022; // cn -> SH1
            AAA_LT_NOTIFY_PORT = 8024; // 无配置转发，是用IP的，则PM发送
            AAA_IVY_NOTIFY_PORT = 8025;

            // PM_START_IN_PORT = 3027;

            // [8031, 8038] cn -> SH1
            // [8039, 8046] cn -> SH2
            PM_START_OUT_PORT = 8031;
        }
        else if (purpose == Purpose.tds_android)
        {
            // [3051, 3100] 如被占用则换一个
            // [8101, 8200] 如被占用：非PM则换一个，PM则不使用那个ID的PM
            LOC_PORT = 3051;
            MONITOR_PORT = 3052;
            DB_ACCOUNT_PORT = 3053;
            DB_PLAYER_PORT = 3054;
            DB_LOG_PORT = 3055;
            AAA_IN_PORT = 3056;

            AAA_OUT_PORT = 8101;
            WEB_PORT = 8102;
            AAA_LT_NOTIFY_PORT = 8104;
            AAA_IVY_NOTIFY_PORT = 8105;

            // PM_START_IN_PORT = 3057;

            PM_START_OUT_PORT = 8111;
        }
        else if (purpose == Purpose.tds_android_en)
        {
            // [3101, 3199] 如被占用则换一个
            // [8201, 8399] 如被占用：非PM则换一个，PM则不使用那个ID的PM
            LOC_PORT = 3101;
            MONITOR_PORT = 3102;
            DB_ACCOUNT_PORT = 3103;
            DB_PLAYER_PORT = 3104;
            DB_LOG_PORT = 3105;
            AAA_IN_PORT = 3106;

            AAA_OUT_PORT = 8201; // global -> HK1
            WEB_PORT = 8202; // global -> HK1
            AAA_LT_NOTIFY_PORT = 8204; // global -> HK1
            AAA_IVY_NOTIFY_PORT = 8205; // global -> HK1  // 这个数不能改，填在 ivy 包里的

            // PM_START_IN_PORT = 3107;

            // [8211, 8214] global -> HK1
            PM_START_OUT_PORT = 8211;
        }
        else
        {
            throw new System.Exception("unknown purpose");
        }
    }

    public static int getOutPortByServerId(int serverId)
    {
        if (serverId == AAA_ID)
        {
            return AAA_OUT_PORT;
        }
        else if (serverId >= PM_START_ID && serverId <= PM_END_ID)
        {
            return PM_START_OUT_PORT + (serverId - PM_START_ID);
        }
        else if (serverId >= BM_START_ID && serverId <= BM_END_ID)
        {
            return BM_START_OUT_PORT + (serverId - BM_START_ID);
        }
        else
        {
            return 0;
        }
    }
    public static int getInPortByServerId(int serverId)
    {
        if (serverId == LOC_ID)
        {
            return LOC_PORT;
        }
        else if (serverId == AAA_ID)
        {
            return AAA_IN_PORT;
        }
        else if (serverId == WEB_ID)
        {
            return WEB_PORT;
        }
        else if (serverId == DB_ACCOUNT_ID)
        {
            return DB_ACCOUNT_PORT;
        }
        else if (serverId == DB_PLAYER_ID)
        {
            return DB_PLAYER_PORT;
        }
        else if (serverId == DB_LOG_ID)
        {
            return DB_LOG_PORT;
        }
        else if (serverId == LOBBY_ID)
        {
            return LOBBY_IN_PORT;
        }
        else
        {
            // throw new Exception();
            return 0;
        }
    }
}