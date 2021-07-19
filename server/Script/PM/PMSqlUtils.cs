using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMSqlUtils : SqlUtils<PMServer>
    {
        private Task<MyResponse> QueryDBPlayer(MsgType msgType, object msg)
        {
            return this.server.tcpClientScript.sendToServerAsync(ServerConst.DB_PLAYER_ID, msgType, msg);
        }

        public async Task<MyResponse> queryPlayerAsync(int playerId)
        {
            var msg = new MsgQueryPlayerById { playerId = playerId };
            return await this.QueryDBPlayer(MsgType.DBQueryPlayerById, msg);
        }

        private string createInsertQueryStr(PMPlayer player, List<string> fields, List<object> values)
        {
            var L = fields.Count;
            if (L == 0)
            {
                return null;
            }
            if (L != values.Count)
            {
                this.server.logger.Error("createInsertQueryStr fields.length != values.length");
                return null;
            }

            List<string> buffer = new List<string>();
            buffer.Add("INSERT INTO player (id,");//) VALUES (" + player.id + ",");
            for (int i = 0; i < L; i++)
            {
                buffer.Add(fields[i]);
                if (i < L - 1)
                {
                    buffer.Add(",");
                }
            }
            buffer.Add(") VALUES (" + player.id + ",");
            for (int i = 0; i < L; i++)
            {
                buffer.Add("@" + i);
                if (i < L - 1)
                {
                    buffer.Add(",");
                }
            }
            buffer.Add(")");

            var queryStr = string.Join(null, buffer.ToArray());
            return queryStr;
        }

        public void SavePlayer(PMPlayer player, MsgSavePlayer msg)
        {
            this.QueryDBPlayer(MsgType.DBSavePlayer, msg);
        }

        public Task<MyResponse> SavePlayerAsync(PMPlayer player, MsgSavePlayer msg)
        {
            return this.server.tcpClientScript.sendToServerAsync(ServerConst.DB_PLAYER_ID, MsgType.DBSavePlayer, msg);
        }

        // 仅用于新玩家
        public async Task<MyResponse> InsertPlayerAsync(PMPlayer player)
        {
            var msg = new MsgInsertPlayer
            {
                player = new SqlTablePlayer
                {
                    id = player.id,
                }
            };

            return await this.server.tcpClientScript.sendToServerAsync(ServerConst.DB_PLAYER_ID, MsgType.DBInsertPlayer, msg);
        }

        public async Task<MyResponse> insertPayiOSYield(int playerId, string env, int id, string productId, string bundleId, int quantity, string transactionId, string originalTransactionId, int purchaseDateMs, int expiresDateMs)
        {
            var msg = new MsgInsertPayiOS
            {
                playerId = playerId,
                env = env,
                id = id,
                productId = productId,
                bundleId = bundleId,
                quantity = quantity,
                transactionId = transactionId,
                originalTransactionId = originalTransactionId,
                purchaseDateMs = purchaseDateMs,
                expiresDateMs = expiresDateMs,
            };
            return await this.server.tcpClientScript.sendToServerAsync(ServerConst.DB_PLAYER_ID, MsgType.DBInsertPayiOS, msg);
        }

        private PMSqlHelpObject NewHelpObject(PMPlayer player)
        {
            var obj = new PMSqlHelpObject();
            obj.player = player;
            obj.fields = new List<string>();
            obj.values = new List<object>();
            return obj;
        }

        // public void save(PMPlayerInfo player, Action<PMSqlHelpObject> fun)
        // {
        //     var obj = this.newHelpObject(player);
        //     fun(obj);
        //     this.saveFieldBatch(player, obj.fields, obj.values);
        // }

        // public async Task<MyResponse> saveYield(PMPlayerInfo player, Action<PMSqlHelpObject> fun)
        // {
        //     var obj = this.newHelpObject(player);
        //     fun(obj);
        //     return await this.saveFieldBatchYield(player, obj.fields, obj.values);
        // }
    }
}