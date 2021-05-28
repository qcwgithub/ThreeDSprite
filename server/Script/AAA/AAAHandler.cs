
using Data;

namespace Script
{
    public abstract class AAAHandler : Handler<AAAServer>
    {
        public AAAData aaaData { get { return this.server.aaaData; } }
        public AAAScript aaaScript { get { return this.server.aaaScript; } }
    }
}