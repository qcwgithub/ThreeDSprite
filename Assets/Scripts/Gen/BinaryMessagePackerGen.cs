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
                default:
                    throw new Exception();
                    //break;
            }
            return obj;
        }
    }
}