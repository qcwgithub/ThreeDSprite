using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;

namespace Script
{
    public partial class BinaryMessagePacker
    {
        
        byte[] PackBody(MessageCode messageCode, object obj)
        {
            byte[] bytes = null;
            switch (messageCode)
            {
                case MessageCode.BMBattleInfo: bytes = MessagePackSerializer.Serialize<BMBattleInfo>((BMBattleInfo)obj); break;
                case MessageCode.BMMsgDebugGetCharacterPosition: bytes = MessagePackSerializer.Serialize<BMMsgDebugGetCharacterPosition>((BMMsgDebugGetCharacterPosition)obj); break;
                case MessageCode.BMMsgMove: bytes = MessagePackSerializer.Serialize<BMMsgMove>((BMMsgMove)obj); break;
                case MessageCode.BMMsgPlayerLogin: bytes = MessagePackSerializer.Serialize<BMMsgPlayerLogin>((BMMsgPlayerLogin)obj); break;
                case MessageCode.BMPlayerInfo: bytes = MessagePackSerializer.Serialize<BMPlayerInfo>((BMPlayerInfo)obj); break;
                case MessageCode.BMResDebugGetCharacterPosition: bytes = MessagePackSerializer.Serialize<BMResDebugGetCharacterPosition>((BMResDebugGetCharacterPosition)obj); break;
                case MessageCode.BMResMove: bytes = MessagePackSerializer.Serialize<BMResMove>((BMResMove)obj); break;
                case MessageCode.BMResPlayerLogin: bytes = MessagePackSerializer.Serialize<BMResPlayerLogin>((BMResPlayerLogin)obj); break;
                case MessageCode.btBattle: bytes = MessagePackSerializer.Serialize<btBattle>((btBattle)obj); break;
                case MessageCode.MCharacter: bytes = MessagePackSerializer.Serialize<MCharacter>((MCharacter)obj); break;
                case MessageCode.MsgChangeChannel: bytes = MessagePackSerializer.Serialize<MsgChangeChannel>((MsgChangeChannel)obj); break;
                case MessageCode.MsgChangeName: bytes = MessagePackSerializer.Serialize<MsgChangeName>((MsgChangeName)obj); break;
                case MessageCode.MsgChangePortrait: bytes = MessagePackSerializer.Serialize<MsgChangePortrait>((MsgChangePortrait)obj); break;
                case MessageCode.MsgEnterBattle: bytes = MessagePackSerializer.Serialize<MsgEnterBattle>((MsgEnterBattle)obj); break;
                case MessageCode.MsgGetVipDailyReward: bytes = MessagePackSerializer.Serialize<MsgGetVipDailyReward>((MsgGetVipDailyReward)obj); break;
                case MessageCode.MsgLoginAAA: bytes = MessagePackSerializer.Serialize<MsgLoginAAA>((MsgLoginAAA)obj); break;
                case MessageCode.MsgLoginPM: bytes = MessagePackSerializer.Serialize<MsgLoginPM>((MsgLoginPM)obj); break;
                case MessageCode.MsgNull: bytes = MessagePackSerializer.Serialize<MsgNull>((MsgNull)obj); break;
                case MessageCode.MsgPay: bytes = MessagePackSerializer.Serialize<MsgPay>((MsgPay)obj); break;
                case MessageCode.MsgPayIvyStart: bytes = MessagePackSerializer.Serialize<MsgPayIvyStart>((MsgPayIvyStart)obj); break;
                case MessageCode.MsgPayLtStart: bytes = MessagePackSerializer.Serialize<MsgPayLtStart>((MsgPayLtStart)obj); break;
                case MessageCode.MsgSyncProfile: bytes = MessagePackSerializer.Serialize<MsgSyncProfile>((MsgSyncProfile)obj); break;
                case MessageCode.MsgUploadProfile: bytes = MessagePackSerializer.Serialize<MsgUploadProfile>((MsgUploadProfile)obj); break;
                case MessageCode.Profile: bytes = MessagePackSerializer.Serialize<Profile>((Profile)obj); break;
                case MessageCode.PurchasedItem: bytes = MessagePackSerializer.Serialize<PurchasedItem>((PurchasedItem)obj); break;
                case MessageCode.ResChangeChannel: bytes = MessagePackSerializer.Serialize<ResChangeChannel>((ResChangeChannel)obj); break;
                case MessageCode.ResChangeName: bytes = MessagePackSerializer.Serialize<ResChangeName>((ResChangeName)obj); break;
                case MessageCode.ResChangePortrait: bytes = MessagePackSerializer.Serialize<ResChangePortrait>((ResChangePortrait)obj); break;
                case MessageCode.ResEnterBattle: bytes = MessagePackSerializer.Serialize<ResEnterBattle>((ResEnterBattle)obj); break;
                case MessageCode.ResGetVipDailyReward: bytes = MessagePackSerializer.Serialize<ResGetVipDailyReward>((ResGetVipDailyReward)obj); break;
                case MessageCode.ResLoginAAA: bytes = MessagePackSerializer.Serialize<ResLoginAAA>((ResLoginAAA)obj); break;
                case MessageCode.ResLoginPM: bytes = MessagePackSerializer.Serialize<ResLoginPM>((ResLoginPM)obj); break;
                case MessageCode.ResMisc: bytes = MessagePackSerializer.Serialize<ResMisc>((ResMisc)obj); break;
                case MessageCode.ResMysqlError: bytes = MessagePackSerializer.Serialize<ResMysqlError>((ResMysqlError)obj); break;
                case MessageCode.ResPay: bytes = MessagePackSerializer.Serialize<ResPay>((ResPay)obj); break;
                case MessageCode.ResPayIvyStart: bytes = MessagePackSerializer.Serialize<ResPayIvyStart>((ResPayIvyStart)obj); break;
                case MessageCode.ResPayLtStart: bytes = MessagePackSerializer.Serialize<ResPayLtStart>((ResPayLtStart)obj); break;
                case MessageCode.ResSyncProfile: bytes = MessagePackSerializer.Serialize<ResSyncProfile>((ResSyncProfile)obj); break;
                case MessageCode.MsgType: bytes = MessagePackSerializer.Serialize<MsgType>((MsgType)obj); break;
                case MessageCode._PRS: bytes = MessagePackSerializer.Serialize<_PRS>((_PRS)obj); break;
                case MessageCode.IvyPayResult: bytes = MessagePackSerializer.Serialize<IvyPayResult>((IvyPayResult)obj); break;
                case MessageCode.LobbyBattleInfo: bytes = MessagePackSerializer.Serialize<LobbyBattleInfo>((LobbyBattleInfo)obj); break;
                case MessageCode.Loc: bytes = MessagePackSerializer.Serialize<Loc>((Loc)obj); break;
                case MessageCode.LtPayResult: bytes = MessagePackSerializer.Serialize<LtPayResult>((LtPayResult)obj); break;
                case MessageCode.MsgAAAAction: bytes = MessagePackSerializer.Serialize<MsgAAAAction>((MsgAAAAction)obj); break;
                case MessageCode.MsgBMAlive: bytes = MessagePackSerializer.Serialize<MsgBMAlive>((MsgBMAlive)obj); break;
                case MessageCode.MsgBMCreateBattle: bytes = MessagePackSerializer.Serialize<MsgBMCreateBattle>((MsgBMCreateBattle)obj); break;
                case MessageCode.MsgBMPlayerEnter: bytes = MessagePackSerializer.Serialize<MsgBMPlayerEnter>((MsgBMPlayerEnter)obj); break;
                case MessageCode.MsgBMPlayerExit: bytes = MessagePackSerializer.Serialize<MsgBMPlayerExit>((MsgBMPlayerExit)obj); break;
                case MessageCode.MsgDBChangeChannel: bytes = MessagePackSerializer.Serialize<MsgDBChangeChannel>((MsgDBChangeChannel)obj); break;
                case MessageCode.MsgDBInsertAccount: bytes = MessagePackSerializer.Serialize<MsgDBInsertAccount>((MsgDBInsertAccount)obj); break;
                case MessageCode.MsgDestroyPlayer: bytes = MessagePackSerializer.Serialize<MsgDestroyPlayer>((MsgDestroyPlayer)obj); break;
                case MessageCode.MsgInsertPayiOS: bytes = MessagePackSerializer.Serialize<MsgInsertPayiOS>((MsgInsertPayiOS)obj); break;
                case MessageCode.MsgInsertPlayer: bytes = MessagePackSerializer.Serialize<MsgInsertPlayer>((MsgInsertPlayer)obj); break;
                case MessageCode.MsgKeepAliveToLoc: bytes = MessagePackSerializer.Serialize<MsgKeepAliveToLoc>((MsgKeepAliveToLoc)obj); break;
                case MessageCode.MsgLobbyCreateBattle: bytes = MessagePackSerializer.Serialize<MsgLobbyCreateBattle>((MsgLobbyCreateBattle)obj); break;
                case MessageCode.MsgLobbyDestroyBattle: bytes = MessagePackSerializer.Serialize<MsgLobbyDestroyBattle>((MsgLobbyDestroyBattle)obj); break;
                case MessageCode.MsgLobbyPlayerEnterBattle: bytes = MessagePackSerializer.Serialize<MsgLobbyPlayerEnterBattle>((MsgLobbyPlayerEnterBattle)obj); break;
                case MessageCode.MsgLobbyPlayerExitBattle: bytes = MessagePackSerializer.Serialize<MsgLobbyPlayerExitBattle>((MsgLobbyPlayerExitBattle)obj); break;
                case MessageCode.MsgLocBroadcast: bytes = MessagePackSerializer.Serialize<MsgLocBroadcast>((MsgLocBroadcast)obj); break;
                case MessageCode.MsgLocBroadcastMsgAAAAction: bytes = MessagePackSerializer.Serialize<MsgLocBroadcastMsgAAAAction>((MsgLocBroadcastMsgAAAAction)obj); break;
                case MessageCode.MsgLocBroadcastMsgPMAction: bytes = MessagePackSerializer.Serialize<MsgLocBroadcastMsgPMAction>((MsgLocBroadcastMsgPMAction)obj); break;
                case MessageCode.MsgLocReportLoc: bytes = MessagePackSerializer.Serialize<MsgLocReportLoc>((MsgLocReportLoc)obj); break;
                case MessageCode.MsgLocRequestLoc: bytes = MessagePackSerializer.Serialize<MsgLocRequestLoc>((MsgLocRequestLoc)obj); break;
                case MessageCode.MsgLogChangeChannel: bytes = MessagePackSerializer.Serialize<MsgLogChangeChannel>((MsgLogChangeChannel)obj); break;
                case MessageCode.MsgLogPlayerLogin: bytes = MessagePackSerializer.Serialize<MsgLogPlayerLogin>((MsgLogPlayerLogin)obj); break;
                case MessageCode.MsgLogPlayerLogout: bytes = MessagePackSerializer.Serialize<MsgLogPlayerLogout>((MsgLogPlayerLogout)obj); break;
                case MessageCode.MsgOnClose: bytes = MessagePackSerializer.Serialize<MsgOnClose>((MsgOnClose)obj); break;
                case MessageCode.MsgOnConnect: bytes = MessagePackSerializer.Serialize<MsgOnConnect>((MsgOnConnect)obj); break;
                case MessageCode.MsgOnDisconnect: bytes = MessagePackSerializer.Serialize<MsgOnDisconnect>((MsgOnDisconnect)obj); break;
                case MessageCode.MsgPlayerSCSave: bytes = MessagePackSerializer.Serialize<MsgPlayerSCSave>((MsgPlayerSCSave)obj); break;
                case MessageCode.MsgPMAction: bytes = MessagePackSerializer.Serialize<MsgPMAction>((MsgPMAction)obj); break;
                case MessageCode.MsgPMAlive: bytes = MessagePackSerializer.Serialize<MsgPMAlive>((MsgPMAlive)obj); break;
                case MessageCode.MsgPreparePlayerLogin: bytes = MessagePackSerializer.Serialize<MsgPreparePlayerLogin>((MsgPreparePlayerLogin)obj); break;
                case MessageCode.MsgQueryAccountByChannel: bytes = MessagePackSerializer.Serialize<MsgQueryAccountByChannel>((MsgQueryAccountByChannel)obj); break;
                case MessageCode.MsgQueryAccountByPlayerId: bytes = MessagePackSerializer.Serialize<MsgQueryAccountByPlayerId>((MsgQueryAccountByPlayerId)obj); break;
                case MessageCode.MsgQueryAccountForChangeChannel: bytes = MessagePackSerializer.Serialize<MsgQueryAccountForChangeChannel>((MsgQueryAccountForChangeChannel)obj); break;
                case MessageCode.MsgQueryAccountUpdatePlayerId: bytes = MessagePackSerializer.Serialize<MsgQueryAccountUpdatePlayerId>((MsgQueryAccountUpdatePlayerId)obj); break;
                case MessageCode.MsgQueryPlayerById: bytes = MessagePackSerializer.Serialize<MsgQueryPlayerById>((MsgQueryPlayerById)obj); break;
                case MessageCode.MsgReloadScript: bytes = MessagePackSerializer.Serialize<MsgReloadScript>((MsgReloadScript)obj); break;
                case MessageCode.MsgRunScript: bytes = MessagePackSerializer.Serialize<MsgRunScript>((MsgRunScript)obj); break;
                case MessageCode.MsgSavePlayer: bytes = MessagePackSerializer.Serialize<MsgSavePlayer>((MsgSavePlayer)obj); break;
                case MessageCode.MsgSendDestroyPlayer: bytes = MessagePackSerializer.Serialize<MsgSendDestroyPlayer>((MsgSendDestroyPlayer)obj); break;
                case MessageCode.ResBMAlive: bytes = MessagePackSerializer.Serialize<ResBMAlive>((ResBMAlive)obj); break;
                case MessageCode.ResBMPlayerEnter: bytes = MessagePackSerializer.Serialize<ResBMPlayerEnter>((ResBMPlayerEnter)obj); break;
                case MessageCode.ResDBQueryAccountPlayerId: bytes = MessagePackSerializer.Serialize<ResDBQueryAccountPlayerId>((ResDBQueryAccountPlayerId)obj); break;
                case MessageCode.ResLobbyCreateBattle: bytes = MessagePackSerializer.Serialize<ResLobbyCreateBattle>((ResLobbyCreateBattle)obj); break;
                case MessageCode.ResLobbyPlayerEnterBattle: bytes = MessagePackSerializer.Serialize<ResLobbyPlayerEnterBattle>((ResLobbyPlayerEnterBattle)obj); break;
                case MessageCode.ResLocRequestLoc: bytes = MessagePackSerializer.Serialize<ResLocRequestLoc>((ResLocRequestLoc)obj); break;
                case MessageCode.ResPMAlive: bytes = MessagePackSerializer.Serialize<ResPMAlive>((ResPMAlive)obj); break;
                case MessageCode.ResPreparePlayerLogin: bytes = MessagePackSerializer.Serialize<ResPreparePlayerLogin>((ResPreparePlayerLogin)obj); break;
                case MessageCode.ResQueryAccount: bytes = MessagePackSerializer.Serialize<ResQueryAccount>((ResQueryAccount)obj); break;
                case MessageCode.ResQueryPlayer: bytes = MessagePackSerializer.Serialize<ResQueryPlayer>((ResQueryPlayer)obj); break;
                case MessageCode.SqlTableAccount: bytes = MessagePackSerializer.Serialize<SqlTableAccount>((SqlTableAccount)obj); break;
                case MessageCode.SqlTablePlayer: bytes = MessagePackSerializer.Serialize<SqlTablePlayer>((SqlTablePlayer)obj); break;
                default:
                    throw new Exception();
                    //break;
            }
            return bytes;
        }
        object UnpackBody(MessageCode messageCode, byte[] buffer, int offset, int count)
        {
            object obj = null;
            var readonlyMemory = new ReadOnlyMemory<byte>(buffer, offset, count);
            switch (messageCode)
            {
                case MessageCode.BMBattleInfo: obj = MessagePackSerializer.Deserialize<BMBattleInfo>(readonlyMemory); break;
                case MessageCode.BMMsgDebugGetCharacterPosition: obj = MessagePackSerializer.Deserialize<BMMsgDebugGetCharacterPosition>(readonlyMemory); break;
                case MessageCode.BMMsgMove: obj = MessagePackSerializer.Deserialize<BMMsgMove>(readonlyMemory); break;
                case MessageCode.BMMsgPlayerLogin: obj = MessagePackSerializer.Deserialize<BMMsgPlayerLogin>(readonlyMemory); break;
                case MessageCode.BMPlayerInfo: obj = MessagePackSerializer.Deserialize<BMPlayerInfo>(readonlyMemory); break;
                case MessageCode.BMResDebugGetCharacterPosition: obj = MessagePackSerializer.Deserialize<BMResDebugGetCharacterPosition>(readonlyMemory); break;
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
                case MessageCode.MsgQueryAccountUpdatePlayerId: obj = MessagePackSerializer.Deserialize<MsgQueryAccountUpdatePlayerId>(readonlyMemory); break;
                case MessageCode.MsgQueryPlayerById: obj = MessagePackSerializer.Deserialize<MsgQueryPlayerById>(readonlyMemory); break;
                case MessageCode.MsgReloadScript: obj = MessagePackSerializer.Deserialize<MsgReloadScript>(readonlyMemory); break;
                case MessageCode.MsgRunScript: obj = MessagePackSerializer.Deserialize<MsgRunScript>(readonlyMemory); break;
                case MessageCode.MsgSavePlayer: obj = MessagePackSerializer.Deserialize<MsgSavePlayer>(readonlyMemory); break;
                case MessageCode.MsgSendDestroyPlayer: obj = MessagePackSerializer.Deserialize<MsgSendDestroyPlayer>(readonlyMemory); break;
                case MessageCode.ResBMAlive: obj = MessagePackSerializer.Deserialize<ResBMAlive>(readonlyMemory); break;
                case MessageCode.ResBMPlayerEnter: obj = MessagePackSerializer.Deserialize<ResBMPlayerEnter>(readonlyMemory); break;
                case MessageCode.ResDBQueryAccountPlayerId: obj = MessagePackSerializer.Deserialize<ResDBQueryAccountPlayerId>(readonlyMemory); break;
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