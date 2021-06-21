using System.Collections.Generic;

namespace Data
{
    public class MsgPreparePlayerLogin : ISerializable
    {
        public int playerId;
        public string token;
        public string channel;
        public string channelUserId;
        public string userName;
    }
    public class ResPreparePlayerLogin : ISerializable
    {
        public bool needUploadProfile;
    }

    public enum ReloadType
    {
        // -class X { server: Server; }
        // -server.x = new X();
        serverScript,
        // -class Y { server: Server; }
        // -server.dispatcher.add(y);
        handler,
        // -ONLY for PM
        // -class Z { scripts; configs; init() {} }
        // -server.z = new Z();
        gameScript,
    }

    public class MsgReloadScript : ISerializable
    {
        public string dllPath;
    }

    public class MsgRunScript : ISerializable
    {
        public string script;
    }

    public enum MyDBValueType
    {
        DateTime = 1,
    }
    public abstract class MsgDBQuery
    {
        public string queryStr;
        public List<object> values;
        public Dictionary<int, int> valueTypes;//?: any; // null, or: index of values -> MyDBValueType
        public int expectedAffectedRows;//?: number;
        
        public int expectedCount;
    }

    public class MsgQueryAccountByPlayerId : ISerializable
    {
        public int playerId;
    }

    public class MsgQueryAccountByChannel : ISerializable
    {
        public string channel;
        public string channelUserId;
    }

    public class MsgQueryAccountForChangeChannel : ISerializable
    {
        public string channel;
        public string channelUserId;
        public string notExistChannel;
        public string notExistChannelUserId;
    }

    public class ResQueryAccount : ISerializable
    {
        public List<SqlTableAccount> list;
    }

    public class MsgDBInsertAccount : ISerializable
    {
        public SqlTableAccount accountInfo;
    }
    
    public class MsgDBChangeChannel : ISerializable
    {
        public string channel1;
        public string channelUserId1;
        public string channel2;
        public string channelUserId2;
        public string userInfo;
    }

    public class MsgQueryPlayerById : ISerializable
    {
        public int playerId;
    }

    public class ResQueryPlayer : ISerializable
    {
        public List<SqlTablePlayer> list;
    }

    public class MsgLogPlayerLogin : ISerializable
    {
        public int playerId;
    }
    public class MsgLogPlayerLogout : ISerializable
    {
        public int playerId;
    }
    public class MsgLogChangeChannel : ISerializable
    {
        public int playerId;
        public string channel1;
        public string channelUserId1;
        public string channel2;
        public string channelUserId2;
    }

    public class MsgSavePlayer : ISerializable {}

    public class MsgInsertPlayer : ISerializable
    {
        public SqlTablePlayer player;
    }
    public class MsgInsertPayiOS : ISerializable
    {
        public int playerId;
        public string env;
        public int id;
        public string productId;
        public string bundleId;
        public int quantity;
        public string transactionId;
        public string originalTransactionId;
        public int purchaseDateMs;
        public int expiresDateMs;
    }

    public class LtPayResult
    {
        public string amount;     // Y 金额（分），(游戏需要做校验)
        public string channelNo;  // Y 渠道编号
        public string extInfo;    // Y 游戏客户端透传字段，原样返回
        public string gameOrderNo;// Y 游戏订单号
        public string thirdNo;    // Y 第三方订单号
        public string productId;  // N 商品ID，iOS渠道必填（游戏需要做校验）
        public string status;     // Y 值为“success”时表示成功订单（默认都返回success）
                                  // Y 签名字段，规则如下：
                                  // 1、通知参数（sign、extInfo除外）按字典顺序从小到大排序，以key=value格式拼接，参数间以&符号连接；如：amount=1&status=success
                                  // 2、参数拼接后再拼接上&key=签名密钥（密钥找运维提供）
                                  // 3、所有参数拼接后取MD5值转小写
        public string sign;
    }

    // https://github.com/IvySdk/android
    // example: 
    // country=cn&sku=com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame2&payId=201&orderId=GPA.3313-2873-5329-79636&purchaseTime=1609757087955&purchaseToken=fibmeibcadnclopgnblekbmm.AO-J1OwnmY1DDejE8CW9qjch5u1ZC2o-1EhfAtXMw__iR4zGDGN7Mpt2el20WihRf8EzM_BVbIhPb87H9yMglGojfjq7qGFAUBDgdF4c4MGEX2I2VLex2anQ22UtQvw3NBC3essMVqmDI0xy_WrggPrV-YcvRbkbiQ&purchaseState=1&uuid=A1696C93-4465-4489-A32C-8508CFE1D1DC&packageName=com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame&jsonData=%7B%22orderId%22%3A%22GPA.3313-2873-5329-79636%22%2C%22packageName%22%3A%22com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame%22%2C%22productId%22%3A%22com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame2%22%2C%22purchaseTime%22%3A1609757087955%2C%22purchaseState%22%3A0%2C%22purchaseToken%22%3A%22fibmeibcadnclopgnblekbmm.AO-J1OwnmY1DDejE8CW9qjch5u1ZC2o-1EhfAtXMw__iR4zGDGN7Mpt2el20WihRf8EzM_BVbIhPb87H9yMglGojfjq7qGFAUBDgdF4c4MGEX2I2VLex2anQ22UtQvw3NBC3essMVqmDI0xy_WrggPrV-YcvRbkbiQ%22%2C%22acknowledged%22%3Afalse%7D&signature=xQcOgGHDzM7%2FKFNrRQ94q5PJj9BEft3rjZ7HowqU2WvMpEXK1PRgFXovkB4weoOHFtSLNIYR6uBB8wOqiQ9OqNNcENy%2BZIa6ZtTl6gPLiJ3uHyk00G6Vo5tmG4fwqzQ9WDgOms9dLcLNDHLwMnqSIAodZ1OTJCIEUm7MeYQDkh2msmCiRGSc3n7PZQgHLr4CRNy0SKCmq9b0nbAW9jmhLyQsQa8YlnrcZZbCe8KE%2BPGj739nrz9IdXgumUOKLcVk0BSd8O8%2FwH0bmEvP%2BesGRH0lQDFzNXQ%2F97yqRNzVIRxtE4nRG8H1qZVImuETH%2BrMAYqv%2BGJtNMmFyzzVKxo1gw%3D%3D&sku_json=%7B%22id%22%3A%22com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame2%22%2C%22type%22%3A%22inapp%22%2C%22price%22%3A%22HK%248.00%22%2C%22original_price%22%3A%22HK%248.00%22%2C%22price_amount%22%3A8%2C%22original_price_amount%22%3A8%2C%22currency%22%3A%22HKD%22%2C%22title%22%3A%2230%20Gems%20%28Merge%20Mall%20Town%3A%20Decorate%20Home%2C%20Classic%20Idle%20Game%29%22%2C%22desc%22%3A%2230%20Gems%22%2C%22usd%22%3A0%7D&appid=51005
    public class IvyPayResult
    {
        public string country;
        public string sku;    // google sku（应该是商品ID）
        public string payId;  // 我们的 IapConfig.itemId
        public string orderId;    // 别人的 orderId，"GPA.3313-2873-5329-79636"
        public string purchaseTime;   // 时间戳
        public string purchaseToken;  // purchaseToken可以用来自行向google服务器监听购买的通知，比如退款等
        public string purchaseState;  // 应为‘1’
        public string uuid;   // UUID,用处不知道
        public string packageName;
        public string jsonData;
        public string signature;
        public string sku_json;
        public string appid;
        public string payload;    // 透传字段，是我们自己的订单号
    }

    public class MsgPlayerSCSave : ISerializable
    {
        public int playerId;
        public string place;
    }

    public class MsgSendDestroyPlayer : ISerializable
    {
        public int playerId;
    }
    public class MsgDestroyPlayer : ISerializable
    {
        public int playerId;
        public string place;
    }

    public class _PRS
    {
        public List<int> playerIds;
        public string script;
    }
    public class MsgPMAction : ISerializable
    {
        public _PRS playerRunScript;
        public string allowNewPlayer; // false 表示 AAA 不会分配新玩家到此 PM
        public string allowClientConnect; // false 表示不接受客户端连接
        public string playerDestroyTimeoutS; // 下线后多久清除此玩家
        public string playerSCSaveIntervalS;
        public string destroyAll;
        public List<int> destroyPlayerIds;
    }

    public class MsgPMAlive : ISerializable
    {
        public int id;
        public int playerCount;
        public Loc loc;
        public List<int> playerList;
        public bool allowNewPlayer;
    }

    public class MsgAAAAction : ISerializable
    {
        public _PRS pmPlayerRunScript; // 发送至 PM
        public string active;
        public string destroyAll;
        public List<int> destroyPlayerIds;
    }

    public class MsgOnConnect : ISerializable
    {
        public bool isAcceptor;
        // public bool isServer;
    }
    public class MsgOnDisconnect : ISerializable
    {
        public bool isAcceptor;
        // public bool isServer;
    }
    public class MsgOnClose : ISerializable
    {
        public bool isAcceptor;
        // public bool isServer;
    }

    public class MsgLocBroadcast : ISerializable
    {
        public List<int> ids;
        public MsgType msgType;
        public virtual object getMsg() { return null; }
        public virtual void setMsg(object _msg) {}
    }

    public class MsgLocBroadcastMsgAAAAction : MsgLocBroadcast
    {
        public MsgAAAAction msg;
        public override object getMsg() { return msg; }
        public override void setMsg(object _msg) { msg = (MsgAAAAction)_msg; }
    }
    public class MsgLocBroadcastMsgPMAction : MsgLocBroadcast
    {
        public MsgPMAction msg;
        public override object getMsg() { return msg; }
        public override void setMsg(object _msg) { msg = (MsgPMAction)_msg; }
    }

    public class MsgLocReportLoc : ISerializable
    {
        public int id;
        public Loc loc;
    }

    public class MsgLocRequestLoc : ISerializable
    {
        public List<int> ids;
    }

    public class ResLocRequestLoc : ISerializable
    {
        public List<Loc> locs;
    }

    public class MsgKeepAliveToLoc : ISerializable
    {
        public bool isListen;
        public bool isServer;
    }

    public class ResPMAlive : ISerializable
    {
        public bool requirePlayerList;
    }

    public class MsgBMAlive : ISerializable
    {
        public int bmId;
        public Loc loc;
        public int battleCount;
        public List<LobbyBattleInfo> battles;
        public bool allowNewBattle;
    }

    public class ResBMAlive : ISerializable
    {
        public bool requireBattleList;
    }

    public class MsgLobbyDestroyBattle :ISerializable
    {
        public int bmId;
        public int battleId;
        public List<int> playerIds;
    }

    public class MsgLobbyCreateBattle : ISerializable
    {
        
    }

    public class ResLobbyCreateBattle : ISerializable
    {
        public int bmId;
        public int battleId;

    }
    public class MsgLobbyPlayerEnterBattle : ISerializable
    {
        public int playerId;
    }
    public class ResLobbyPlayerEnterBattle : ISerializable
    {
        public bool alreadyInBattle;
        public int bmId;
        public int battleId;
        public string bmIp;
        public int bmPort;
    }

    public class MsgBMCreateBattle : ISerializable
    {
        public int battleId;
    }

    public class MsgBMPlayerEnter : ISerializable
    {
        public int playerId;
        public int battleId;
    }
    public class ResBMPlayerEnter : ISerializable
    {
        public string token;
    }

    public class MsgLobbyPlayerExitBattle : ISerializable
    {
        public int playerId;
    }
    public class MsgBMPlayerExit : ISerializable
    {
        public int battleId;
        public int playerId;
    }
}