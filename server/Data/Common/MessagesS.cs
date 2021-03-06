using System.Collections.Generic;
using MessagePack;
using System.Numerics;

namespace Data
{
    [MessagePackObject]
    public class MsgPreparePlayerLogin
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public string token;
        [Key(2)]
        public string channel;
        [Key(3)]
        public string channelUserId;
        [Key(4)]
        public string userName;
    }
    [MessagePackObject]
    public class ResPreparePlayerLogin
    {
        [Key(0)]
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

    [MessagePackObject]
    public class MsgReloadScript
    {
        [Key(0)]
        public string dllPath;
    }

    [MessagePackObject]
    public class MsgRunScript
    {
        [Key(0)]
        public string script;
    }

    public enum MyDBValueType
    {
        DateTime = 1,
    }

/*
    [MessagePackObject]
    public abstract class MsgDBQuery
    {
        [Key(0)]
        public string queryStr;
        [Key(1)]
        public List<object> values;
        [Key(2)]
        public Dictionary<int, int> valueTypes;//?: any; // null, or: index of values -> MyDBValueType
        [Key(3)]
        public int expectedAffectedRows;//?: number;

        [Key(4)]
        public int expectedCount;
    }
*/
    [MessagePackObject]
    public class MsgQueryAccountByPlayerId
    {
        [Key(0)]
        public int playerId;
    }

    
    [MessagePackObject]
    public class MsgQueryAccountUpdatePlayerId
    {
        [Key(0)]
        public int playerId;
    }

    [MessagePackObject]
    public class MsgQueryAccountByChannel
    {
        [Key(0)]
        public string channel;
        [Key(1)]
        public string channelUserId;
    }

    [MessagePackObject]
    public class MsgQueryAccountForChangeChannel
    {
        [Key(0)]
        public string channel;
        [Key(1)]
        public string channelUserId;
        [Key(2)]
        public string notExistChannel;
        [Key(3)]
        public string notExistChannelUserId;
    }

    [MessagePackObject]
    public class ResQueryAccount
    {
        [Key(0)]
        public List<SqlTableAccount> list;
    }

    [MessagePackObject]
    public class MsgDBInsertAccount
    {
        [Key(0)]
        public SqlTableAccount accountInfo;
    }

    [MessagePackObject]
    public class MsgDBChangeChannel
    {
        [Key(0)]
        public string channel1;
        [Key(1)]
        public string channelUserId1;
        [Key(2)]
        public string channel2;
        [Key(3)]
        public string channelUserId2;
        [Key(4)]
        public string userInfo;
    }

    [MessagePackObject]
    public class ResDBQueryAccountPlayerId
    {
        [Key(0)]
        public int playerId;
    }

    [MessagePackObject]
    public class MsgQueryPlayerById
    {
        [Key(0)]
        public int playerId;
    }

    [MessagePackObject]
    public class ResQueryPlayer
    {
        [Key(0)]
        public List<SqlTablePlayer> list;
    }

    [MessagePackObject]
    public class MsgLogPlayerLogin
    {
        [Key(0)]
        public int playerId;
    }
    [MessagePackObject]
    public class MsgLogPlayerLogout
    {
        [Key(0)]
        public int playerId;
    }
    [MessagePackObject]
    public class MsgLogChangeChannel
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public string channel1;
        [Key(2)]
        public string channelUserId1;
        [Key(3)]
        public string channel2;
        [Key(4)]
        public string channelUserId2;
        [Key(5)]
        public int? abcdtest;
    }

    [MessagePackObject]
    public class MsgSavePlayer
    {
        [Key(0)]
        public int playerId;

        #region MsgSavePlayer Auto

        [Key(1)]
        public int? level;
        [Key(2)]
        public BigInteger money;
        [Key(3)]
        public int? diamond;
        [Key(4)]
        public string portrait;
        [Key(5)]
        public string userName;
        [Key(6)]
        public int? characterConfigId;

        #endregion MsgSavePlayer Auto
    }

    [MessagePackObject]
    public class MsgInsertPlayer
    {
        [Key(0)]
        public SqlTablePlayer player;
    }
    [MessagePackObject]
    public class MsgInsertPayiOS
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public string env;
        [Key(2)]
        public int id;
        [Key(3)]
        public string productId;
        [Key(4)]
        public string bundleId;
        [Key(5)]
        public int quantity;
        [Key(6)]
        public string transactionId;
        [Key(7)]
        public string originalTransactionId;
        [Key(8)]
        public int purchaseDateMs;
        [Key(9)]
        public int expiresDateMs;
    }

    [MessagePackObject]
    public class LtPayResult
    {
        [Key(0)]
        public string amount;     // Y ??????????????????(?????????????????????)
        [Key(1)]
        public string channelNo;  // Y ????????????
        [Key(2)]
        public string extInfo;    // Y ??????????????????????????????????????????
        [Key(3)]
        public string gameOrderNo;// Y ???????????????
        [Key(4)]
        public string thirdNo;    // Y ??????????????????
        [Key(5)]
        public string productId;  // N ??????ID???iOS???????????????????????????????????????
        [Key(6)]
        public string status;     // Y ?????????success??????????????????????????????????????????success???
                                  // Y ??????????????????????????????
                                  // 1??????????????????sign???extInfo????????????????????????????????????????????????key=value???????????????????????????&?????????????????????amount=1&status=success
                                  // 2??????????????????????????????&key=???????????????????????????????????????
                                  // 3???????????????????????????MD5????????????
        [Key(7)]
        public string sign;
    }

    // https://github.com/IvySdk/android
    // example: 
    // country=cn&sku=com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame2&payId=201&orderId=GPA.3313-2873-5329-79636&purchaseTime=1609757087955&purchaseToken=fibmeibcadnclopgnblekbmm.AO-J1OwnmY1DDejE8CW9qjch5u1ZC2o-1EhfAtXMw__iR4zGDGN7Mpt2el20WihRf8EzM_BVbIhPb87H9yMglGojfjq7qGFAUBDgdF4c4MGEX2I2VLex2anQ22UtQvw3NBC3essMVqmDI0xy_WrggPrV-YcvRbkbiQ&purchaseState=1&uuid=A1696C93-4465-4489-A32C-8508CFE1D1DC&packageName=com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame&jsonData=%7B%22orderId%22%3A%22GPA.3313-2873-5329-79636%22%2C%22packageName%22%3A%22com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame%22%2C%22productId%22%3A%22com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame2%22%2C%22purchaseTime%22%3A1609757087955%2C%22purchaseState%22%3A0%2C%22purchaseToken%22%3A%22fibmeibcadnclopgnblekbmm.AO-J1OwnmY1DDejE8CW9qjch5u1ZC2o-1EhfAtXMw__iR4zGDGN7Mpt2el20WihRf8EzM_BVbIhPb87H9yMglGojfjq7qGFAUBDgdF4c4MGEX2I2VLex2anQ22UtQvw3NBC3essMVqmDI0xy_WrggPrV-YcvRbkbiQ%22%2C%22acknowledged%22%3Afalse%7D&signature=xQcOgGHDzM7%2FKFNrRQ94q5PJj9BEft3rjZ7HowqU2WvMpEXK1PRgFXovkB4weoOHFtSLNIYR6uBB8wOqiQ9OqNNcENy%2BZIa6ZtTl6gPLiJ3uHyk00G6Vo5tmG4fwqzQ9WDgOms9dLcLNDHLwMnqSIAodZ1OTJCIEUm7MeYQDkh2msmCiRGSc3n7PZQgHLr4CRNy0SKCmq9b0nbAW9jmhLyQsQa8YlnrcZZbCe8KE%2BPGj739nrz9IdXgumUOKLcVk0BSd8O8%2FwH0bmEvP%2BesGRH0lQDFzNXQ%2F97yqRNzVIRxtE4nRG8H1qZVImuETH%2BrMAYqv%2BGJtNMmFyzzVKxo1gw%3D%3D&sku_json=%7B%22id%22%3A%22com.mergemalltown.decoratehome.hotelmanager.shopmall.idlegame2%22%2C%22type%22%3A%22inapp%22%2C%22price%22%3A%22HK%248.00%22%2C%22original_price%22%3A%22HK%248.00%22%2C%22price_amount%22%3A8%2C%22original_price_amount%22%3A8%2C%22currency%22%3A%22HKD%22%2C%22title%22%3A%2230%20Gems%20%28Merge%20Mall%20Town%3A%20Decorate%20Home%2C%20Classic%20Idle%20Game%29%22%2C%22desc%22%3A%2230%20Gems%22%2C%22usd%22%3A0%7D&appid=51005
    
    [MessagePackObject]
    public class IvyPayResult
    {
        [Key(0)]
        public string country;
        [Key(1)]
        public string sku;    // google sku??????????????????ID???
        [Key(2)]
        public string payId;  // ????????? IapConfig.itemId
        [Key(3)]
        public string orderId;    // ????????? orderId???"GPA.3313-2873-5329-79636"
        [Key(4)]
        public string purchaseTime;   // ?????????
        [Key(5)]
        public string purchaseToken;  // purchaseToken?????????????????????google????????????????????????????????????????????????
        [Key(6)]
        public string purchaseState;  // ?????????1???
        [Key(7)]
        public string uuid;   // UUID,???????????????
        [Key(8)]
        public string packageName;
        [Key(9)]
        public string jsonData;
        [Key(10)]
        public string signature;
        [Key(11)]
        public string sku_json;
        [Key(12)]
        public string appid;
        [Key(13)]
        public string payload;    // ??????????????????????????????????????????
    }

    [MessagePackObject]
    public class MsgPlayerSCSave
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public string place;
    }

    [MessagePackObject]
    public class MsgSendDestroyPlayer
    {
        [Key(0)]
        public int playerId;
    }
    [MessagePackObject]
    public class MsgDestroyPlayer
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public string place;
    }

    [MessagePackObject]
    public class _PRS
    {
        [Key(0)]
        public List<int> playerIds;
        [Key(1)]
        public string script;
    }

    [MessagePackObject]
    public class MsgPMAction
    {
        [Key(0)]
        public _PRS playerRunScript;
        [Key(1)]
        public string allowNewPlayer; // false ?????? AAA ??????????????????????????? PM
        [Key(2)]
        public string allowClientConnect; // false ??????????????????????????????
        [Key(3)]
        public string playerDestroyTimeoutS; // ??????????????????????????????
        [Key(4)]
        public string playerSCSaveIntervalS;
        [Key(5)]
        public string destroyAll;
        [Key(6)]
        public List<int> destroyPlayerIds;
    }

    [MessagePackObject]
    public class MsgPMAlive
    {
        [Key(0)]
        public int pmId;
        [Key(1)]
        public int playerCount;
        [Key(2)]
        public Loc loc;
        [Key(3)]
        public List<int> playerList;
        [Key(4)]
        public bool allowNewPlayer;
    }

    [MessagePackObject]
    public class MsgAAAAction
    {
        [Key(0)]
        public _PRS pmPlayerRunScript; // ????????? PM
        [Key(1)]
        public string active;
        [Key(2)]
        public string destroyAll;
        [Key(3)]
        public List<int> destroyPlayerIds;
    }

    [MessagePackObject]
    public class MsgOnConnect
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }
    [MessagePackObject]
    public class MsgOnDisconnect
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }
    [MessagePackObject]
    public class MsgOnClose
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }

    [MessagePackObject]
    public class MsgLocBroadcast
    {
        [Key(0)]
        public List<int> serverIds;
        [Key(1)]
        public MsgType msgType;
        public virtual object getMsg() { return null; }
        public virtual void setMsg(object _msg) { }
    }

    [MessagePackObject]
    public class MsgLocBroadcastMsgAAAAction : MsgLocBroadcast
    {
        [Key(2)]
        public MsgAAAAction msg;
        public override object getMsg() { return msg; }
        public override void setMsg(object _msg) { msg = (MsgAAAAction)_msg; }
    }

    [MessagePackObject]
    public class MsgLocBroadcastMsgPMAction : MsgLocBroadcast
    {
        [Key(2)]
        public MsgPMAction msg;
        public override object getMsg() { return msg; }
        public override void setMsg(object _msg) { msg = (MsgPMAction)_msg; }
    }

    [MessagePackObject]
    public class MsgLocReportLoc
    {
        [Key(0)]
        public int serverId;
        [Key(1)]
        public Loc loc;
    }

    [MessagePackObject]
    public class MsgLocRequestLoc
    {
        [Key(0)]
        public List<int> serverIds;
    }

    [MessagePackObject]
    public class ResLocRequestLoc
    {
        [Key(0)]
        public List<Loc> locs;
    }

    [MessagePackObject]
    public class MsgKeepAliveToLoc
    {
        [Key(0)]
        public bool isListen;
        [Key(1)]
        public bool isServer;
    }

    [MessagePackObject]
    public class ResPMAlive
    {
        [Key(0)]
        public bool requirePlayerList;
    }

    [MessagePackObject]
    public class MsgBMAlive
    {
        [Key(0)]
        public int bmId;
        [Key(1)]
        public Loc loc;
        [Key(2)]
        public int battleCount;
        [Key(3)]
        public List<LobbyBattleInfo> battles;
        [Key(4)]
        public bool allowNewBattle;
    }

    [MessagePackObject]
    public class ResBMAlive
    {
        [Key(0)]
        public bool requireBattleList;
    }

    [MessagePackObject]
    public class MsgLobbyDestroyBattle
    {
        [Key(0)]
        public int bmId;
        [Key(1)]
        public int battleId;
        [Key(2)]
        public List<int> playerIds;
    }

    [MessagePackObject]
    public class MsgLobbyCreateBattle
    {

    }

    [MessagePackObject]
    public class ResLobbyCreateBattle
    {
        [Key(0)]
        public int bmId;
        [Key(1)]
        public int battleId;
    }
    
    [MessagePackObject]
    public class MsgLobbyPlayerEnterBattle
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public int characterConfigId;
    }
    
    [MessagePackObject]
    public class ResLobbyPlayerEnterBattle
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
    public class MsgBMCreateBattle
    {
        [Key(0)]
        public int battleId;
        [Key(1)]
        public int mapId;
    }

    [MessagePackObject]
    public class MsgBMPlayerEnter
    {
        [Key(0)]
        public int playerId;
        [Key(1)]
        public int battleId;
        [Key(2)]
        public int characterConfigId;
    }
    
    [MessagePackObject]
    public class ResBMPlayerEnter
    {
        [Key(0)]
        public string token;
    }

    [MessagePackObject]
    public class MsgLobbyPlayerExitBattle
    {
        [Key(0)]
        public int playerId;
    }
    
    [MessagePackObject]
    public class MsgBMPlayerExit
    {
        [Key(0)]
        public int battleId;
        [Key(1)]
        public int playerId;
    }
}