using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class PMChangeCharacter : PMHandler
    {
        public override MsgType msgType { get { return MsgType.ChangeCharacter; } }
        public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.CastObject<MsgChangeCharacter>(_msg);
            
            PMPlayer player = this.getPlayer(socket);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} player == null!!", this.msgName);
                return ECode.PlayerNotExist;
            }
            
            this.server.logger.Info(this.msgName + ", playerId: " + player.id);
            
            var res = new ResChangeCharacter();
            ECode e = this.server.gameScript.ChangeCharacterCheck(player, msg, res);
            if(e != ECode.Success)
            {
                return e;
            }

            this.server.gameScript.ChangeCharacterExecute(player, msg, res);
            return new MyResponse(e, res);
        }
    }
}