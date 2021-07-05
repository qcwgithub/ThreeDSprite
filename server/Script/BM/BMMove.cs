using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BMMove : BMHandler
    {
        public override MsgType msgType => MsgType.BMMove;
        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            var msg = this.server.castObject<MsgBMMove>(_msg);
            BMPlayerInfo player = this.getPlayer(socket);
            if (player == null)
            {
                this.logger.ErrorFormat("{0} player == null!!", this.msgName);
                return ECode.PlayerNotExist.toTask();
            }

            
            
            ECode e = this.server.moveScript.characterMove(player.character, FVector3.ToVector3(msg.moveDir));
            if (e != ECode.Success)
            {
                return e.toTask();
            }
            
            return ECode.Success.toTask();
        }
    }
}