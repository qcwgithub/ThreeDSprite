using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using log4net.Repository;
using log4net.Appender;
using System.Xml;
using System.Xml.Linq;
using Data;

namespace Script
{
    public class ServerCreation
    {
        public List<Server> Create(DataEntry dataEntry, int version)
        {
            var list = new List<Server>();
            for (int i = 0; i < dataEntry.serverIds.Count; i++)
            {
                int serverId = dataEntry.serverIds[i];

                if (serverId == ServerConst.LOC_ID)
                {
                    var s = new LocServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else if (serverId == ServerConst.AAA_ID)
                {
                    var s = new AAAServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else if (serverId == ServerConst.WEB_ID)
                {
                    continue;
                }
                else if (serverId == ServerConst.MONITOR_ID)
                {                    
                    var s = new MonitorServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                    continue;
                }
                else if (serverId == ServerConst.DB_ACCOUNT_ID)
                {
                    var s = new DBServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else if (serverId == ServerConst.DB_PLAYER_ID)
                {
                    var s = new DBServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else if (serverId == ServerConst.DB_LOG_ID)
                {
                    var s = new DBServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else if (serverId >= ServerConst.PM_START_ID && serverId <= ServerConst.PM_END_ID)
                {
                    var s = new PMServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else if (serverId == ServerConst.LOBBY_ID)
                {
                    var s = new LobbyServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else if (serverId >= ServerConst.BM_START_ID && serverId <= ServerConst.BM_END_ID)
                {
                    var s = new BMServer();
                    s.OnLoad(dataEntry, serverId, version);
                    list.Add(s);
                }
                else
                {
                    continue;
                }
            }
            return list;
        }
    }
}