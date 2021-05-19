using System.Collections.Generic;

public class ResMisc
{
    public int oldSocketTimestamp;
}

public class MsgLoginAAA
{
    // 版本号，如果与服务器不一致，则不允许登录
    public string version;

    // android | ios | 101
    public string platform;
    // uuid | apple | ...
    public string channel;
    // 如果 channel == uuid 是 uuid
    // 如果 channel == apple，channelUserId 是苹果 id
    public string channelUserId;

    public object verifyData;
    public string oaid;
    public string imei;
}


public class ResLoginAAA
{
    public string channel;
    public string channelUserId;
    public int playerId;
    public int pmId;
    public string pmUrl;
    public string pmToken;
    public bool needUploadProfile;
}

public class MsgChangeChannel
{
    public string channel1;
    public string channelUserId1;

    public string channel2;
    public string channelUserId2;
    public object verifyData2;

    public int playerId; // 由PM赋值，客户端传0
}
public class ResChangeChannel
{
    public bool channel2Exist; // 由 AAA 赋值，true表示要更换的渠道已经存在了，客户端此次不是绑定，而是需要用此号重新登录一下游戏
    public string userName; // 由 AAA 赋值
    public int loginReward; // 由 PM 赋值
}

public class MsgLoginPM
{
    public bool isReconnect;
    public int playerId;
    public string token;
    public CProfile profile;
    public int timestamp;
}

public class ResLoginPM
{
    public int id;
    public bool keepSyncProfile;
    public CProfile profile; // 重连时为 null
    public int timeMs;
    public int timezoneOffset;

    // 以下几个只需要在重连时使用
    public int offlineBonusTime; // 不管是不是重连，都有值。如果不是重连，值等于 profile.offlineBonus.time
    public int totalGameTimeMs; // 不管是不是重连，都有值。
    public int totalLoginTimes; // 不管是不是重连，都有值。
    public int diamond; // 不管是不是重连，都有值。
    public int badge; // 不管是不是重连，都有值。
    public IapLeitingInfo[] ltProducts; // 不管是不是重连，都有值。
    public string payNotifyUri; // 不管是不是重连，都有值。


    public int act_durationH;
    public int act_daySplitH;

    public CProfile updateProfile; // 如果有值，表示把里面的字段更新到客户端
    public string script;
}

///////////////////////////////////////////////////////////

public class MsgUploadProfile
{
    public CProfile profile;
}

// 与 ProfileType 一一对应
public class MsgSyncProfile
{
    
}
public class ResSyncProfile
{
}
public class MsgChangeName
{
    public string name;
}
public class ResChangeName
{

}

public class MsgChangePortrait
{
    public string portrait;
}
public class ResChangePortrait
{

}

public class MsgGetVipDailyReward
{

}
public class ResGetVipDailyReward
{
    public int todayMs;
    public int addDiamond;
}

// 雷霆--发起支付请求，其他是在服务器创建个订单号
public class MsgPayLtStart
{
    public string productId;
    public string fen; // 分
}
public class ResPayLtStart
{
    public string orderId; // 当 err = ECode.Success
    public IapLeitingInfo[] ltProducts; // 当 err = ECode.ProductPriceRefreshed
}

public class MsgPayIvyStart
{
    public int id;
}
public class ResPayIvyStart
{
    public string orderId; // 当 err = ECode.Success
}

public class MsgPay
{
    public string receipt;
}

public class PurchasedItem
{
    public bool duplicated;
    public string productId;  // 由于一个 id 可以对应多个 productId（不同平台），因此这里传 productId
    public string transactionId;
    public int addDiamond;
    public int addGiftVoucher;
}
public class ResPay
{
    // 月卡也可能在 items 列表中
    public List<PurchasedItem> items;
    // 月卡
    public bool monthlyCardUpdated;
    public int purchaseDateMs;
    public int expiresDateMs;

    // profile.numbers
    // index - newValue - index - newValue
    public int[] numberUpdates;
}
public class ResMysqlError
{
    /**
     * Either a MySQL server error (e.g. "ER_ACCESS_DENIED_ERROR"),
     * a node.js error (e.g. "ECONNREFUSED") or an internal error
     * (e.g. "PROTOCOL_CONNECTION_LOST").
     */
    public string code;

    /**
     * The error number for the error code
     */
    public int errno;
}

public class MsgActivityStart
{

}