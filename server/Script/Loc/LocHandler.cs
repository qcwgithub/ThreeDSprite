using Data;

namespace Script
{
    public abstract class LocHandler : Handler<LocServer>
    {
        public LocData locData { get { return this.server.locData; } }
        public LocScript locScript { get { return this.server.locScript; } }
    }
}