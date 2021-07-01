
using Data;

namespace Script
{
    public abstract class BMHandler : Handler<BMServer>
    {
        public BMPlayerInfo getPlayer(TcpClientData socket)
        {
            object obj = this.server.tcpClientScript.getPlayer(socket);
            return (obj == null ? null : (BMPlayerInfo)obj);
        }
    }
}