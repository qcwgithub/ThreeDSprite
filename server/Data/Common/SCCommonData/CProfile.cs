// 这几个地方要同时修改，保持一致，以 CProfile 为中心
// CProfile
// CProfile.ensure
// ProfileStorage *2      x
// SyncProfileComponent
// PMSqlUtils
// PMSyncProfile
// PMScript.decodePlayer
// PMScript.newPlayer

/*
类型0 只存在客户端，服务器不知道这些数据的存在。如果重装游戏则消失
类型1 服务器全权负责。任何字段的变化都必须经过服务器校验后才可以修改
类型2 变化时不需要经过服务器校验，但是要立刻通知服务器，如果通知失败，下次进游戏会回档
类型3 与类型1类似，但服务器只负责部分数据

*/

// 字段不要初始化值，因为希望 new CProfile() 得到一个空对象
public class CProfile
{
    //#region autoFields >>>>>>>>自动生成区域开始

    //// p
    public string portrait; //31/51 server

    //// u
    public string userID; //48/51 server
    public string userName; //49/51 server


    //#endregion autoFields <<<<<<<<自动生成区域结束

    public static CProfile ensure(CProfile p, string userName)
    {
        if (p == null)
        {
            p = new CProfile();
        }

        return p;
    }
}