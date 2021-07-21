using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgNull
    {

    }
    
    [MessagePackObject]
    public class ResMisc
    {
        [Key(0)]
        public int oldSocketTimestamp;
    }

    [MessagePackObject]
    public class MsgLoginAAA
    {
        // 版本号，如果与服务器不一致，则不允许登录
        [Key(0)]
        public string version;

        // android | ios | 101
        [Key(1)]
        public string platform;
        // uuid | apple | ...
        [Key(2)]
        public string channel;
        // 如果 channel == uuid 是 uuid
        // 如果 channel == apple，channelUserId 是苹果 id
        [Key(3)]
        public string channelUserId;

        [IgnoreMember]
        public Dictionary<string, object> verifyData;
        [Key(4)]
        public string oaid;
        [Key(5)]
        public string imei;
    }


    [MessagePackObject]
    public class ResLoginAAA
    {
        [Key(0)]
        public string channel;
        [Key(1)]
        public string channelUserId;
        [Key(2)]
        public int playerId;
        [Key(3)]
        public int pmId;
        [Key(4)]
        public string pmIp;
        [Key(5)]
        public int pmPort;
        [Key(6)]
        public string pmToken;
        [Key(7)]
        public bool needUploadProfile;
    }

    [MessagePackObject]
    public class MsgChangeChannel
    {
        [Key(0)]
        public string channel1;
        [Key(1)]
        public string channelUserId1;

        [Key(2)]
        public string channel2;
        [Key(3)]
        public string channelUserId2;
        [IgnoreMember]
        public Dictionary<string, object> verifyData2;

        [Key(4)]
        public int playerId; // 由PM赋值，客户端传0
    }
    [MessagePackObject]
    public class ResChangeChannel
    {
        [Key(0)]
        public bool channel2Exist; // 由 AAA 赋值，true表示要更换的渠道已经存在了，客户端此次不是绑定，而是需要用此号重新登录一下游戏
        [Key(1)]
        public string userName; // 由 AAA 赋值
        [Key(2)]
        public int loginReward; // 由 PM 赋值
    }

    [MessagePackObject]
    public class MsgLoginPM
    {
        [Key(0)]
        public bool isReconnect;
        [Key(1)]
        public int playerId;
        [Key(2)]
        public string token;
        [Key(3)]
        public int timestamp;
    }

    [MessagePackObject]
    public class ResLoginPM
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public bool keepSyncProfile;
        [Key(2)]
        public Profile profile; // 重连时为 null
        [Key(3)]
        public int timeMs;
        [Key(4)]
        public int timezoneOffset;

        // 以下几个只需要在重连时使用
        [Key(5)]
        public int offlineBonusTime; // 不管是不是重连，都有值。如果不是重连，值等于 profile.offlineBonus.time
        [Key(6)]
        public int totalGameTimeMs; // 不管是不是重连，都有值。
        [Key(7)]
        public int totalLoginTimes; // 不管是不是重连，都有值。
        [Key(8)]
        public int diamond; // 不管是不是重连，都有值。
        //--------------------------------public IapLeitingInfo[] ltProducts; // 不管是不是重连，都有值。
        [Key(9)]
        public string payNotifyUri; // 不管是不是重连，都有值。
    }

    ///////////////////////////////////////////////////////////

    [MessagePackObject]
    public class MsgUploadProfile
    {
        //--------------------------------public CProfile profile;
    }

    // 与 ProfileType 一一对应
    [MessagePackObject]
    public class MsgSyncProfile
    {

    }
    [MessagePackObject]
    public class ResSyncProfile
    {
    }
    [MessagePackObject]
    public class MsgChangeName
    {
        [Key(0)]
        public string name;
    }
    [MessagePackObject]
    public class ResChangeName
    {

    }

    [MessagePackObject]
    public class MsgChangePortrait
    {
        [Key(0)]
        public string portrait;
    }
    [MessagePackObject]
    public class ResChangePortrait
    {

    }

    [MessagePackObject]
    public class MsgGetVipDailyReward
    {

    }
    [MessagePackObject]
    public class ResGetVipDailyReward
    {
        [Key(0)]
        public int todayMs;
        [Key(1)]
        public int addDiamond;
    }

    // 雷霆--发起支付请求，其他是在服务器创建个订单号
    [MessagePackObject]
    public class MsgPayLtStart
    {
        [Key(0)]
        public string productId;
        [Key(1)]
        public string fen; // 分
    }
    [MessagePackObject]
    public class ResPayLtStart
    {
        [Key(0)]
        public string orderId; // 当 err = ECode.Success
        //--------------------------------public IapLeitingInfo[] ltProducts; // 当 err = ECode.ProductPriceRefreshed
    }

    [MessagePackObject]
    public class MsgPayIvyStart
    {
        [Key(0)]
        public int id;
    }
    [MessagePackObject]
    public class ResPayIvyStart
    {
        [Key(0)]
        public string orderId; // 当 err = ECode.Success
    }

    [MessagePackObject]
    public class MsgPay
    {
        [Key(0)]
        public string receipt;
    }

    [MessagePackObject]
    public class PurchasedItem
    {
        [Key(0)]
        public bool duplicated;
        [Key(1)]
        public string productId;  // 由于一个 id 可以对应多个 productId（不同平台），因此这里传 productId
        [Key(2)]
        public string transactionId;
        [Key(3)]
        public int addDiamond;
        [Key(4)]
        public int addGiftVoucher;
    }
    [MessagePackObject]
    public class ResPay
    {
        // 月卡也可能在 items 列表中
        [Key(0)]
        public List<PurchasedItem> items;
        // 月卡
        [Key(1)]
        public bool monthlyCardUpdated;
        [Key(2)]
        public int purchaseDateMs;
        [Key(3)]
        public int expiresDateMs;

        // profile.numbers
        // index - newValue - index - newValue
        [Key(4)]
        public List<int> numberUpdates;
    }
    [MessagePackObject]
    public class ResMysqlError
    {
        /**
         * Either a MySQL server error (e.g. "ER_ACCESS_DENIED_ERROR"),
         * a node.js error (e.g. "ECONNREFUSED") or an internal error
         * (e.g. "PROTOCOL_CONNECTION_LOST").
         */
        [Key(0)]
        public string code;

        /**
         * The error number for the error code
         */
        [Key(1)]
        public int errno;
    }

    [MessagePackObject]
    public class MsgEnterBattle
    {

    }
    [MessagePackObject]
    public class ResEnterBattle
    {
        [Key(0)]
        public bool alreadyInBattle;
        [Key(1)]
        public int bmId;
        [Key(2)]
        public int battleId;
        [Key(3)]
        public string bmIp;
        [Key(4)]
        public int bmPort;
        [Key(5)]
        public int mapId;
    }

    [MessagePackObject]
    public class MsgChangeCharacter
    {
        [Key(0)]
        public int characterConfigId;
    }

    [MessagePackObject]
    public class ResChangeCharacter
    {
        [Key(0)]
        public int characterConfigId;
    }
}

