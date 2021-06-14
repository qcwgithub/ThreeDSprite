using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMKeepAliveToLobby : BMHandler
    {
        public override MsgType msgType { get { return MsgType.BMKeepAliveToLobby; } }
        public override async Task<MyResponse> handle(TcpClientData socket/* null */, object _msg /* null */)
        { 
            var alive = this.server.bmData.alive;
            if (!this.server.tcpClientScript.isServerConnected(ServerConst.LOBBY_ID))
            {
                alive.count = 10;
                return ECode.Success;
            }

            if (alive.first || alive.requireBattleList)
            {

            }
            else
            {
                // 保持连接的情况下，10秒一次
                alive.count++;
                if (alive.count < 10)
                {
                    // this.server.logger.Info("(skip) keep alive to lobby V" + this.server.scriptDllVersion);
                    return ECode.Success;
                }
            }
            // this.server.logger.Info("keep alive to lobby V" + this.server.scriptDllVersion);

            alive.count = 0;
            alive.first = false;

            List<int> playerList = null;
            if (alive.requireBattleList)
            {
                this.logger.Info("alive.requireBattleList = true");
                alive.requireBattleList = false;
                playerList = new List<int>();
                foreach (var kv in this.server.bmData.battleInfos)
                {
                    playerList.Add(kv.Key);
                }
            }

            var msgAlive = new MsgBMAlive()
            {
                bmId = this.baseData.id,
                loc = this.server.myLoc(),
                allowNewBattle = this.server.bmData.allowNewBattle,
            };
            var r = await this.server.tcpClientScript.sendToServerAsync(ServerConst.LOBBY_ID, MsgType.LobbyOnBMAlive, msgAlive);

            if (r.err != ECode.Success)
            {
                this.logger.Error("BattleKeepAliveToLobby error: " + r.err);
            }
            else
            {
                this.server.bmData.lobbyReady = true;
                var resBattleAlive = this.server.castObject<ResBMAlive>(r.res);
                if (resBattleAlive.requireBattleList)
                {
                    alive.requireBattleList = true;
                }
            }

            return ECode.Success;
        }


        public override MyResponse postHandle(object socket, object msg, MyResponse r)
        {
            this.server.setTimer(1, this.msgType, null);
            return r;
        }
    }
}