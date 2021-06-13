
using Data;

namespace Script
{
    public abstract class MonitorHandler : Handler<MonitorServer>
    {
        public MonitorData monitorData { get { return this.server.monitorData; } }
    }
}