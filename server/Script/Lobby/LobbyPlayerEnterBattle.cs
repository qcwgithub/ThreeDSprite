using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Script
{
    public class LobbyPlayerEnterBattle : LobbyHandler
    {
        public override MsgType msgType => MsgType.LobbyPlayerEnterBattle;

        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            LobbyBMInfo bmInfo = null;
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
                return ECode.NoAvailableBM;
            }

            LobbyBattleInfo battleInfo = null;
            foreach (var kv in bmInfo.battles)
            {
                battleInfo = kv.Value;
                break;
            }


            var msg = this.server.castObject<MsgLobbyPlayerEnterBattle>(_msg);
            LobbyPlayerInfo lobbyPlayerInfo;
            if (this.server.lobbyData.playerInfos.TryGetValue(msg.playerId, out lobbyPlayerInfo))
            {
                return ECode.PlayerAlreadyInBattle;
            }
            
            var msg2 = new MsgBMPlayerEnter();
            msg2.playerId = msg.playerId;
            var r = await this.server.tcpClientScript.sendToServerAsync(bmInfo.bmId, MsgType.BMPlayerEnter, msg2);
            if (r.err != ECode.Success)
            {
                return r.err;
            }

           battleInfo.playerIds.Add(msg.playerId);
           return ECode.Success;
        }
    }
}