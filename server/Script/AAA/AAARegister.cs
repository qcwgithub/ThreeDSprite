namespace Script
{
    public class AAARegister : BaseRegister<AAAServer>
    {
        public override void register(AAAServer server)
        {
            base.register(server);

            server.dispatcher.addHandler(new AAAStart());
            server.dispatcher.addHandler(new AAATest());
            server.dispatcher.addHandler(new AAAOnPMAlive());
            server.dispatcher.addHandler(new AAALoadPlayerId());
            server.dispatcher.addHandler(new AAAChangeChannel());
            server.dispatcher.addHandler(new AAAPlayerLogin());
            server.dispatcher.addHandler(new AAADestroyPlayer());
            server.dispatcher.addHandler(new AAAGetSummary());
            server.dispatcher.addHandler(new AAAShutdown());
            server.dispatcher.addHandler(new AAAAction());
        }
    }
}