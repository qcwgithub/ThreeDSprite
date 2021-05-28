namespace Script
{
    public class LocRegister : BaseRegister<LocServer>
    {
        public override void register(LocServer server)
        {
            base.register(server);

            server.dispatcher.addHandler(new LocStart());
            server.dispatcher.addHandler(new LocOnDisconnect());
            server.dispatcher.addHandler(new LocReportLoc());
            server.dispatcher.addHandler(new LocRequestLoc());
            server.dispatcher.addHandler(new LocBroadcast());
            server.dispatcher.addHandler(new LocGetSummary());
        }
    }
}