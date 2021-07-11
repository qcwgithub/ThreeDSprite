using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;

namespace Script
{
    public partial class BinaryMessagePacker
    {
        
        object UnpackBody(MessageCode messageCode, byte[] buffer, int offset, int count)
        {
            object obj = null;
            var readonlyMemory = new ReadOnlyMemory<byte>(buffer, offset, count);
            switch (messageCode)
            {
                case MessageCode.BMBattleInfo: obj = MessagePackSerializer.Deserialize<BMBattleInfo>(readonlyMemory); break;
                case MessageCode.BMMsgMove: obj = MessagePackSerializer.Deserialize<BMMsgMove>(readonlyMemory); break;
                case MessageCode.BMMsgPlayerLogin: obj = MessagePackSerializer.Deserialize<BMMsgPlayerLogin>(readonlyMemory); break;
                case MessageCode.BMPlayerInfo: obj = MessagePackSerializer.Deserialize<BMPlayerInfo>(readonlyMemory); break;
                case MessageCode.BMResMove: obj = MessagePackSerializer.Deserialize<BMResMove>(readonlyMemory); break;
                case MessageCode.BMResPlayerLogin: obj = MessagePackSerializer.Deserialize<BMResPlayerLogin>(readonlyMemory); break;
                case MessageCode.btBattle: obj = MessagePackSerializer.Deserialize<btBattle>(readonlyMemory); break;
                case MessageCode.MCharacter: obj = MessagePackSerializer.Deserialize<MCharacter>(readonlyMemory); break;
                case MessageCode.MsgChangeChannel: obj = MessagePackSerializer.Deserialize<MsgChangeChannel>(readonlyMemory); break;
                case MessageCode.MsgChangeName: obj = MessagePackSerializer.Deserialize<MsgChangeName>(readonlyMemory); break;
                case MessageCode.MsgChangePortrait: obj = MessagePackSerializer.Deserialize<MsgChangePortrait>(readonlyMemory); break;
                case MessageCode.MsgEnterBattle: obj = MessagePackSerializer.Deserialize<MsgEnterBattle>(readonlyMemory); break;
                case MessageCode.MsgGetVipDailyReward: obj = MessagePackSerializer.Deserialize<MsgGetVipDailyReward>(readonlyMemory); break;
                case MessageCode.MsgLoginAAA: obj = MessagePackSerializer.Deserialize<MsgLoginAAA>(readonlyMemory); break;
                case MessageCode.MsgLoginPM: obj = MessagePackSerializer.Deserialize<MsgLoginPM>(readonlyMemory); break;
                case MessageCode.MsgNull: obj = MessagePackSerializer.Deserialize<MsgNull>(readonlyMemory); break;
                case MessageCode.MsgPay: obj = MessagePackSerializer.Deserialize<MsgPay>(readonlyMemory); break;
                case MessageCode.MsgPayIvyStart: obj = MessagePackSerializer.Deserialize<MsgPayIvyStart>(readonlyMemory); break;
                case MessageCode.MsgPayLtStart: obj = MessagePackSerializer.Deserialize<MsgPayLtStart>(readonlyMemory); break;
                case MessageCode.MsgSyncProfile: obj = MessagePackSerializer.Deserialize<MsgSyncProfile>(readonlyMemory); break;
                case MessageCode.MsgUploadProfile: obj = MessagePackSerializer.Deserialize<MsgUploadProfile>(readonlyMemory); break;
                case MessageCode.Profile: obj = MessagePackSerializer.Deserialize<Profile>(readonlyMemory); break;
                case MessageCode.PurchasedItem: obj = MessagePackSerializer.Deserialize<PurchasedItem>(readonlyMemory); break;
                case MessageCode.ResChangeChannel: obj = MessagePackSerializer.Deserialize<ResChangeChannel>(readonlyMemory); break;
                case MessageCode.ResChangeName: obj = MessagePackSerializer.Deserialize<ResChangeName>(readonlyMemory); break;
                case MessageCode.ResChangePortrait: obj = MessagePackSerializer.Deserialize<ResChangePortrait>(readonlyMemory); break;
                case MessageCode.ResEnterBattle: obj = MessagePackSerializer.Deserialize<ResEnterBattle>(readonlyMemory); break;
                case MessageCode.ResGetVipDailyReward: obj = MessagePackSerializer.Deserialize<ResGetVipDailyReward>(readonlyMemory); break;
                case MessageCode.ResLoginAAA: obj = MessagePackSerializer.Deserialize<ResLoginAAA>(readonlyMemory); break;
                case MessageCode.ResLoginPM: obj = MessagePackSerializer.Deserialize<ResLoginPM>(readonlyMemory); break;
                case MessageCode.ResMisc: obj = MessagePackSerializer.Deserialize<ResMisc>(readonlyMemory); break;
                case MessageCode.ResMysqlError: obj = MessagePackSerializer.Deserialize<ResMysqlError>(readonlyMemory); break;
                case MessageCode.ResPay: obj = MessagePackSerializer.Deserialize<ResPay>(readonlyMemory); break;
                case MessageCode.ResPayIvyStart: obj = MessagePackSerializer.Deserialize<ResPayIvyStart>(readonlyMemory); break;
                case MessageCode.ResPayLtStart: obj = MessagePackSerializer.Deserialize<ResPayLtStart>(readonlyMemory); break;
                case MessageCode.ResSyncProfile: obj = MessagePackSerializer.Deserialize<ResSyncProfile>(readonlyMemory); break;
                case MessageCode.MsgType: obj = MessagePackSerializer.Deserialize<MsgType>(readonlyMemory); break;
                case MessageCode._PRS: obj = MessagePackSerializer.Deserialize<_PRS>(readonlyMemory); break;
                case MessageCode.IvyPayResult: obj = MessagePackSerializer.Deserialize<IvyPayResult>(readonlyMemory); break;
                case MessageCode.LobbyBattleInfo: obj = MessagePackSerializer.Deserialize<LobbyBattleInfo>(readonlyMemory); break;
                case MessageCode.Loc: obj = MessagePackSerializer.Deserialize<Loc>(readonlyMemory); break;
                case MessageCode.LtPayResult: obj = MessagePackSerializer.Deserialize<LtPayResult>(readonlyMemory); break;
                case MessageCode.MsgAAAAction: obj = MessagePackSerializer.Deserialize<MsgAAAAction>(readonlyMemory); break;
                case MessageCode.MsgBMAlive: obj = MessagePackSerializer.Deserialize<MsgBMAlive>(readonlyMemory); break;
                case MessageCode.MsgBMCreateBattle: obj = MessagePackSerializer.Deserialize<MsgBMCreateBattle>(readonlyMemory); break;
                case MessageCode.MsgBMPlayerEnter: obj = MessagePackSerializer.Deserialize<MsgBMPlayerEnter>(readonlyMemory); break;
                case MessageCode.MsgBMPlayerExit: obj = MessagePackSerializer.Deserialize<MsgBMPlayerExit>(readonlyMemory); break;
                case MessageCode.MsgDBChangeChannel: obj = MessagePackSerializer.Deserialize<MsgDBChangeChannel>(readonlyMemory); break;
                case MessageCode.MsgDBInsertAccount: obj = MessagePackSerializer.Deserialize<MsgDBInsertAccount>(readonlyMemory); break;
                case MessageCode.MsgDestroyPlayer: obj = MessagePackSerializer.Deserialize<MsgDestroyPlayer>(readonlyMemory); break;
                case MessageCode.MsgInsertPayiOS: obj = MessagePackSerializer.Deserialize<MsgInsertPayiOS>(readonlyMemory); break;
                case MessageCode.MsgInsertPlayer: obj = MessagePackSerializer.Deserialize<MsgInsertPlayer>(readonlyMemory); break;
                case MessageCode.MsgKeepAliveToLoc: obj = MessagePackSerializer.Deserialize<MsgKeepAliveToLoc>(readonlyMemory); break;
                case MessageCode.MsgLobbyCreateBattle: obj = MessagePackSerializer.Deserialize<MsgLobbyCreateBattle>(readonlyMemory); break;
                case MessageCode.MsgLobbyDestroyBattle: obj = MessagePackSerializer.Deserialize<MsgLobbyDestroyBattle>(readonlyMemory); break;
                case MessageCode.MsgLobbyPlayerEnterBattle: obj = MessagePackSerializer.Deserialize<MsgLobbyPlayerEnterBattle>(readonlyMemory); break;
                case MessageCode.MsgLobbyPlayerExitBattle: obj = MessagePackSerializer.Deserialize<MsgLobbyPlayerExitBattle>(readonlyMemory); break;
                case MessageCode.MsgLocBroadcast: obj = MessagePackSerializer.Deserialize<MsgLocBroadcast>(readonlyMemory); break;
                case MessageCode.MsgLocBroadcastMsgAAAAction: obj = MessagePackSerializer.Deserialize<MsgLocBroadcastMsgAAAAction>(readonlyMemory); break;
                case MessageCode.MsgLocBroadcastMsgPMAction: obj = MessagePackSerializer.Deserialize<MsgLocBroadcastMsgPMAction>(readonlyMemory); break;
                case MessageCode.MsgLocReportLoc: obj = MessagePackSerializer.Deserialize<MsgLocReportLoc>(readonlyMemory); break;
                case MessageCode.MsgLocRequestLoc: obj = MessagePackSerializer.Deserialize<MsgLocRequestLoc>(readonlyMemory); break;
                case MessageCode.MsgLogChangeChannel: obj = MessagePackSerializer.Deserialize<MsgLogChangeChannel>(readonlyMemory); break;
                case MessageCode.MsgLogPlayerLogin: obj = MessagePackSerializer.Deserialize<MsgLogPlayerLogin>(readonlyMemory); break;
                case MessageCode.MsgLogPlayerLogout: obj = MessagePackSerializer.Deserialize<MsgLogPlayerLogout>(readonlyMemory); break;
                case MessageCode.MsgOnClose: obj = MessagePackSerializer.Deserialize<MsgOnClose>(readonlyMemory); break;
                case MessageCode.MsgOnConnect: obj = MessagePackSerializer.Deserialize<MsgOnConnect>(readonlyMemory); break;
                case MessageCode.MsgOnDisconnect: obj = MessagePackSerializer.Deserialize<MsgOnDisconnect>(readonlyMemory); break;
                case MessageCode.MsgPlayerSCSave: obj = MessagePackSerializer.Deserialize<MsgPlayerSCSave>(readonlyMemory); break;
                case MessageCode.MsgPMAction: obj = MessagePackSerializer.Deserialize<MsgPMAction>(readonlyMemory); break;
                case MessageCode.MsgPMAlive: obj = MessagePackSerializer.Deserialize<MsgPMAlive>(readonlyMemory); break;
                case MessageCode.MsgPreparePlayerLogin: obj = MessagePackSerializer.Deserialize<MsgPreparePlayerLogin>(readonlyMemory); break;
                case MessageCode.MsgQueryAccountByChannel: obj = MessagePackSerializer.Deserialize<MsgQueryAccountByChannel>(readonlyMemory); break;
                case MessageCode.MsgQueryAccountByPlayerId: obj = MessagePackSerializer.Deserialize<MsgQueryAccountByPlayerId>(readonlyMemory); break;
                case MessageCode.MsgQueryAccountForChangeChannel: obj = MessagePackSerializer.Deserialize<MsgQueryAccountForChangeChannel>(readonlyMemory); break;
                case MessageCode.MsgQueryPlayerById: obj = MessagePackSerializer.Deserialize<MsgQueryPlayerById>(readonlyMemory); break;
                case MessageCode.MsgReloadScript: obj = MessagePackSerializer.Deserialize<MsgReloadScript>(readonlyMemory); break;
                case MessageCode.MsgRunScript: obj = MessagePackSerializer.Deserialize<MsgRunScript>(readonlyMemory); break;
                case MessageCode.MsgSavePlayer: obj = MessagePackSerializer.Deserialize<MsgSavePlayer>(readonlyMemory); break;
                case MessageCode.MsgSendDestroyPlayer: obj = MessagePackSerializer.Deserialize<MsgSendDestroyPlayer>(readonlyMemory); break;
                case MessageCode.ResBMAlive: obj = MessagePackSerializer.Deserialize<ResBMAlive>(readonlyMemory); break;
                case MessageCode.ResBMPlayerEnter: obj = MessagePackSerializer.Deserialize<ResBMPlayerEnter>(readonlyMemory); break;
                case MessageCode.ResLobbyCreateBattle: obj = MessagePackSerializer.Deserialize<ResLobbyCreateBattle>(readonlyMemory); break;
                case MessageCode.ResLobbyPlayerEnterBattle: obj = MessagePackSerializer.Deserialize<ResLobbyPlayerEnterBattle>(readonlyMemory); break;
                case MessageCode.ResLocRequestLoc: obj = MessagePackSerializer.Deserialize<ResLocRequestLoc>(readonlyMemory); break;
                case MessageCode.ResPMAlive: obj = MessagePackSerializer.Deserialize<ResPMAlive>(readonlyMemory); break;
                case MessageCode.ResPreparePlayerLogin: obj = MessagePackSerializer.Deserialize<ResPreparePlayerLogin>(readonlyMemory); break;
                case MessageCode.ResQueryAccount: obj = MessagePackSerializer.Deserialize<ResQueryAccount>(readonlyMemory); break;
                case MessageCode.ResQueryPlayer: obj = MessagePackSerializer.Deserialize<ResQueryPlayer>(readonlyMemory); break;
                case MessageCode.SqlTableAccount: obj = MessagePackSerializer.Deserialize<SqlTableAccount>(readonlyMemory); break;
                case MessageCode.SqlTablePlayer: obj = MessagePackSerializer.Deserialize<SqlTablePlayer>(readonlyMemory); break;
                default:
                    throw new Exception();
                    //break;
            }
            return obj;
        }
    }
}