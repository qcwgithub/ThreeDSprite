using Data;

namespace Script
{
    public abstract class PMHandler : Handler<PMServer>
    {
        public PMData data { get { return this.server.pmData; } }

        public PMScript pmScript { get { return this.server.pmScript; } }
        public PMSqlUtils pmSqlUtils { get { return this.server.pmSqlUtils; } }
        public SqlLog sqlLog { get { return this.server.sqlLog; } }

        public PMPlayerInfo getPlayer(TcpClientData socket)
        {
            object obj = this.server.tcpClientScript.getPlayer(socket);
            return (obj == null ? null : (PMPlayerInfo)obj);
        }
    }
}