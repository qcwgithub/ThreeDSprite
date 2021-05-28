
using System.Collections.Generic;
using Data;

namespace Script
{
    public class SqlLog : IScript<Server>
    {
        public Server server { get; set; }
        public BaseScript baseScript { get { return this.server.baseScript; } }

        private void doQuery(string queryStr, List<object> values, int expectedAffectedRows)
        {
            var msg = new MsgDBQuery()
            {
                queryStr = queryStr,
                values = values,
                expectedAffectedRows = expectedAffectedRows,
            };
            this.server.baseData.dbLogSocket.send(MsgType.DBQuery, msg, (e, r) =>
            {
                if (e != ECode.Success)
                {
                    this.server.logger.ErrorFormat("SqlLog.doQuery failed. {0}, {1}", e, queryStr);
                }
            });
        }

        public void player_login(PMPlayerInfo player, bool uploadProfile)
        {
            var queryStr = "INSERT INTO player_login (playerId,level,uploadProfile) VALUES (?,?,?);";
            List<object> values = new List<object> { player.id, 1, uploadProfile };
            this.doQuery(queryStr, values, 1);
        }

        public void player_logout(PMPlayerInfo player)
        {
            var queryStr = "INSERT INTO player_logout (playerId,level) VALUES (?,?);";
            List<object> values = new List<object> { player.id, 1 };
            this.doQuery(queryStr, values, 1);
        }

        public void account_changeChannel(int playerId, string channel1, string channelUserId1, string channel2, string channelUserId2)
        {
            var queryStr = "INSERT INTO account_changeChannel(playerId,channel1,channelUserId1,channel2,channelUserId2) VALUES (?,?,?,?,?);";
            List<object> values = new List<object> { playerId, channel1, channelUserId1, channel2, channelUserId2 };
            this.doQuery(queryStr, values, 1);
        }
    }
}