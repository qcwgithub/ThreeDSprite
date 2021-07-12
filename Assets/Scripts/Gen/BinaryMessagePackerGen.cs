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