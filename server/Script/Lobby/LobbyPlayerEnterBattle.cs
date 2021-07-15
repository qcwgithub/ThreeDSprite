using System;
using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Script
{
    public class LobbyPlayerEnterBattle : LobbyHandler
    {
        public override MsgType msgType => MsgType.LobbyPlayerEnterBattle;

        bool findExistingBattle(out LobbyBMInfo bmInfo, out LobbyBattleInfo battleInfo)
        {
            bmInfo = null;
            battleInfo = null;

            foreach (var kv in this.server.lobbyData.bmInfos)
            {
                if (kv.Value.battles.Count > 0)
                {
                    bmInfo = kv.Value;
                    break;
                }
            }
            if (bmInfo == null)
            {
                return false;
            }

            foreach (var kv in bmInfo.battles)
            {
                battleInfo = kv.Value;
                break;
            }

            return true;
        }

        DateTime battleIdBaseTime = new DateTime(2021, 6, 1);
        int getNextBattleId()
        {
            var now = DateTime.Now;
            if (this.server.lobbyData.battleId == 0)
            {
                this.server.lobbyData.battleId = (int) now.Subtract(this.battleIdBaseTime).TotalSeconds;
            }
            return this.server.lobbyData.battleId++;
        }

        async Task<MyResponse> newBattle()
        {
            LobbyBMInfo bmInfo = null;
            foreach (var kv in this.server.lobbyData.bmInfos)
            {
                var bm = kv.Value;
                if (!bm.allowNewBattle)
                {
                    continue;
                }

                if (!this.server.tcpClientScript.isServerConnected(bm.bmId))
                {
                    continue;
                }

                if (bmInfo == null || bm.battleCount < bmInfo.battleCount)
                {
                    bmInfo = bm;
                }
            }

            if (bmInfo == null)
            {
                this.server.logger.Error("no available bm!");
                return ECode.NoAvailableBM;
            }

            int mapId = 1;

            var msg = new MsgBMCreateBattle();
            msg.battleId = this.getNextBattleId();
            msg.mapId = mapId;
            MyResponse r = await this.server.tcpClientScript.sendToServerAsync(bmInfo.bmId, MsgType.BMNewBattle, msg);
            if (r.err != ECode.Success)
            {
                return r;
            }

            var battleInfo = new LobbyBattleInfo();
            battleInfo.bmId = bmInfo.bmId;
            battleInfo.battleId = msg.battleId;
            battleInfo.playerIds = new List<int>();
            battleInfo.mapId = mapId;
            bmInfo.battles.Add(battleInfo.battleId, battleInfo);

            var res = new ResLobbyCreateBattle();
            res.bmId = bmInfo.bmId;
            res.battleId = battleInfo.battleId;
            return new MyResponse(ECode.Success, res);
        }

        async Task<MyResponse> enterBattle(LobbyBMInfo bmInfo, LobbyBattleInfo battleInfo, int playerId)
        {
            var bmMsg = new MsgBMPlayerEnter();
            bmMsg.playerId = playerId;
            bmMsg.battleId = battleInfo.battleId;
            var r = await this.server.tcpClientScript.sendToServerAsync(bmInfo.bmId, MsgType.BMPlayerEnter, bmMsg);
            if (r.err != ECode.Success)
            {
                return r;
            }
            battleInfo.playerIds.Add(playerId);
            return r;
        }

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            if (this.server.lobbyData.bmInfos.Count == 0)
            {
                return ECode.NoAvailableBM;
            }

            var msg = this.server.castObject<MsgLobbyPlayerEnterBattle>(_msg);
            this.server.logger.Info(this.msgName + ", playerId: " + msg.playerId);

            LobbyBMInfo bmInfo = null;
            LobbyBattleInfo battleInfo = null;
            MyResponse r = null;
            bool alreadyInBattle = false;

            LobbyPlayerInfo lobbyPlayerInfo;
            if (this.server.lobbyData.playerInfos.TryGetValue(msg.playerId, out lobbyPlayerInfo))
            {
                // 玩家已在游戏中
                alreadyInBattle = true;
                bmInfo = this.server.lobbyData.bmInfos[lobbyPlayerInfo.bmId];
                battleInfo = bmInfo.battles[lobbyPlayerInfo.battleId];
            }
            else
            {
                if (!this.findExistingBattle(out bmInfo, out battleInfo))
                {
                    // 没有任何一场战斗可以加入，新建一场战斗
                    r = await this.newBattle();
                    if (r.err != ECode.Success)
                    {
                        return r;
                    }
                    var resLobbyCreateBattle = r.res as ResLobbyCreateBattle;
                    bmInfo = this.server.lobbyData.bmInfos[resLobbyCreateBattle.bmId];
                    battleInfo = bmInfo.battles[resLobbyCreateBattle.battleId];
                }

                r = await this.enterBattle(bmInfo, battleInfo, msg.playerId);
                if (r.err != ECode.Success)
                {
                    return r;
                }

                lobbyPlayerInfo = new LobbyPlayerInfo();
                lobbyPlayerInfo.playerId = msg.playerId;
                lobbyPlayerInfo.bmId = bmInfo.bmId;
                lobbyPlayerInfo.battleId = battleInfo.battleId;
                this.server.lobbyData.playerInfos.Add(msg.playerId, lobbyPlayerInfo);
            }

            var res = new ResLobbyPlayerEnterBattle();
            res.alreadyInBattle = alreadyInBattle;
            res.bmId = bmInfo.bmId;
            res.battleId = lobbyPlayerInfo.battleId;
            res.bmIp = this.server.getKnownLoc(bmInfo.bmId).outIp;
            res.bmPort = this.server.getKnownLoc(bmInfo.bmId).outPort;
            res.mapId = battleInfo.mapId;
            return new MyResponse(ECode.Success, res);
        }
    }
}