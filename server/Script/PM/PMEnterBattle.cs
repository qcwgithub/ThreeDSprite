using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMEnterBattle : PMHandler
    {
        public override MsgType msgType { get { return MsgType.PMEnterBattle; } }
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgEnterBattle>(_msg);
            
            var player = this.getPlayer(socket);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} player == null!!", this.msgName);
                return ECode.PlayerNotExist;
            }
            
            this.server.logger.Info(this.msgName + ", playerId: " + player.id);

            if (!this.server.tcpClientScript.isServerConnected(ServerConst.LOBBY_ID))
            {
                return ECode.LobbyNotConnected;
            }

            var lobbyMsg = new MsgLobbyPlayerEnterBattle();
            lobbyMsg.playerId = player.id;
            MyResponse r = await this.server.tcpClientScript.sendToServerAsync(ServerConst.LOBBY_ID, MsgType.LobbyPlayerEnterBattle, lobbyMsg);
            if (r.err != ECode.Success)
            {
                return r;
            }

            var lobbyRes = r.res as ResLobbyPlayerEnterBattle;
            var res = new ResEnterBattle();
            res.alreadyInBattle = lobbyRes.alreadyInBattle;
            res.bmId = lobbyRes.bmId;
            res.battleId = lobbyRes.battleId;
            res.bmIp = lobbyRes.bmIp;
            res.bmPort = lobbyRes.bmPort;
            res.mapId = lobbyRes.mapId;
            return new MyResponse(ECode.Success, res);
        }
    }
}