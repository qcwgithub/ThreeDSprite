using Data;
using System.IO;

namespace Script
{
    public class MonitorServer : Server
    {
        public MonitorData monitorData
        {
            get
            {
                return (MonitorData)this.data;
            }
        }

        public override void OnLoad(DataEntry dataEntry, int serverId, int version)
        {
            base.OnLoad(dataEntry, serverId, version);
            base.AddHandler<MonitorServer>();

            // this.dispatcher.addHandler(new MonitorStart { server = this });
            this.dispatcher.addHandler(new MonitorOnInput { server = this });
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}