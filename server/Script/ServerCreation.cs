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
                int id = dataEntry.serverIds[i];

                if (id == ServerConst.LOC_ID)
                {
                    var s = new LocServer();
                    s.OnLoad(dataEntry, id, version);
                    list.Add(s);
                }
                else if (id == ServerConst.AAA_ID)
                {
                    var s = new AAAServer();
                    s.OnLoad(dataEntry, id, version);
                    list.Add(s);
                }
                else if (id == ServerConst.WEB_ID)
                {
                    continue;
                }
                else if (id == ServerConst.MONITOR_ID)
                {                    
                    var s = new MonitorServer();
                    s.OnLoad(dataEntry, id, version);
                    list.Add(s);
                    continue;
                }
                else if (id == ServerConst.DB_ACCOUNT_ID)
                {
                    var s = new DBServer();
                    s.OnLoad(dataEntry, id, version);
                    list.Add(s);
                }
                else if (id == ServerConst.DB_PLAYER_ID)
                {
                    var s = new DBServer();
                    s.OnLoad(dataEntry, id, version);
                    list.Add(s);
                }
                else if (id == ServerConst.DB_LOG_ID)
                {
                    var s = new DBServer();
                    s.OnLoad(dataEntry, id, version);
                    list.Add(s);
                }
                else if (id >= ServerConst.PM_START_ID && id <= ServerConst.PM_END_ID)
                {
                    var s = new PMServer();
                    s.OnLoad(dataEntry, id, version);
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