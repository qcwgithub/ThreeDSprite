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

        async Task<MyResponse> enterBattle(LobbyBMInfo bmInfo, LobbyBattleInfo battleInfo, int playerId, int characterConfigId)
        {
            var msg = new MsgBMPlayerEnter();
            msg.playerId = playerId;
            msg.battleId = battleInfo.battleId;
            msg.characterConfigId = characterConfigId;
            var r = await this.server.tcpClientScript.sendToServerAsync(bmInfo.bmId, MsgType.BMPlayerEnter, msg);
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

            var msg = this.server.CastObject<MsgLobbyPlayerEnterBattle>(_msg);
            this.server.logger.Info(this.msgName + ", playerId: " + msg.playerId);

            LobbyBMInfo bmInfo = null;
            LobbyBattleInfo battleInfo = null;
            MyResponse r = null;
            bool alreadyInBattle = false;

            LobbyPlayer lobbyPlayer;
            if (this.server.lobbyData.playerDict.TryGetValue(msg.playerId, out lobbyPlayer))
            {
                // ?????????????????????
                alreadyInBattle = true;
                bmInfo = this.server.lobbyData.bmInfos[lobbyPlayer.bmId];
                battleInfo = bmInfo.battles[lobbyPlayer.battleId];
            }
            else
            {
                if (!this.findExistingBattle(out bmInfo, out battleInfo))
                {
                    // ?????????????????????????????????????????????????????????
                    r = await this.newBattle();
                    if (r.err != ECode.Success)
                    {
                        return r;
                    }
                    var resLobbyCreateBattle = r.res as ResLobbyCreateBattle;
                    bmInfo = this.server.lobbyData.bmInfos[resLobbyCreateBattle.bmId];
                    battleInfo = bmInfo.battles[resLobbyCreateBattle.battleId];
                }

                r = await this.enterBattle(bmInfo, battleInfo, msg.playerId, msg.characterConfigId);
                if (r.err != ECode.Success)
                {
                    return r;
                }

                lobbyPlayer = new LobbyPlayer();
                lobbyPlayer.playerId = msg.playerId;
                lobbyPlayer.bmId = bmInfo.bmId;
                lobbyPlayer.battleId = battleInfo.battleId;
                this.server.lobbyData.playerDict.Add(msg.playerId, lobbyPlayer);
            }

            var res = new ResLobbyPlayerEnterBattle();
            res.alreadyInBattle = alreadyInBattle;
            res.bmId = bmInfo.bmId;
            res.battleId = lobbyPlayer.battleId;
            res.bmIp = this.server.getKnownLoc(bmInfo.bmId).outIp;
            res.bmPort = this.server.getKnownLoc(bmInfo.bmId).outPort;
            res.mapId = battleInfo.mapId;
            return new MyResponse(ECode.Success, res);
        }
    }
}