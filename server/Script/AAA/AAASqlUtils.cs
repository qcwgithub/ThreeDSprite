
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAASqlUtils : SqlUtils<AAAServer>
    {
        private Task<MyResponse> queryAccount(MsgType msgType, object msg)
        {
            return this.server.tcpClientScript.sendToServerAsync(ServerConst.DB_ACCOUNT_ID, msgType, msg);
        }

        public async Task<MyResponse> selectPlayerIdAsync()
        {
            return await this.queryAccount(MsgType.DBQueryPlayerId, null);
        }

        public async Task<MyResponse> updatePlayerIdAsync(int playerId)
        {
            return await this.queryAccount(MsgType.DBUpdatePlayerId, playerId);
        }

        public async Task<MyResponse> queryAccountByPlayerIdAsync(int playerId)
        {
            var msg = new MsgQueryAccountByPlayerId
            {
                playerId = playerId,
            };
            return await this.queryAccount(MsgType.DBQueryAccountByPlayerId, msg);
        }

        public async Task<MyResponse> queryAccountByChannel(string channel, string channelUserId)
        {
            var msg = new MsgQueryAccountByChannel
            {
                channel = channel,
                channelUserId = channelUserId,
            };
            return await this.queryAccount(MsgType.DBQueryAccountByChannel, msg);
        }
        public async Task<MyResponse> queryAccountForChangeChannelAsync(string channel, string channelUserId,
            string notExistChannel, string notExistChannelUserId)
        {
            var msg = new MsgQueryAccountForChangeChannel
            {
                channel = channel,
                channelUserId = channelUserId,
                notExistChannel = notExistChannel,
                notExistChannelUserId = notExistChannelUserId,
            };
            return await this.queryAccount(MsgType.DBQueryAccountForChangeChannel, msg);
        }

        public async Task<MyResponse> insertAccountAsync(SqlTableAccount a)
        {
            var msg = new MsgDBInsertAccount
            {
                accountInfo = a,
            };
            return await this.queryAccount(MsgType.DBInsertAccount, msg);
        }

        // (channel1, channelUserId1) -> (channel2, channelUserId2)
        public async Task<MyResponse> changeChannelYield(string channel1, string channelUserId1, string channel2, string channelUserId2, string userInfo)
        {
            var msg = new MsgDBChangeChannel
            {
                channel1 = channel1,
                channelUserId1 = channelUserId1,
                channel2 = channel2,
                channelUserId2 = channelUserId2,
                userInfo = userInfo,
            };
            return await this.queryAccount(MsgType.DBChangeChannel, msg);
        }
    }
}