
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class LobbySetBMReady : LobbyHandler
    {
        public override MsgType msgType { get { return MsgType.LobbySetBMReady; } }

        public override Task<MyResponse> handle(TcpClientData socket, object _msg)
        {
            this.server.lobbyData.bmReady = true;
            this.server.lobbyData.bmReadyTimer = 0;
            return ECode.Success.toTask();
        }
    }
}