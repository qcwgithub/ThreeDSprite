namespace Script
{
    public class DBRegister : BaseRegister<DBServer>
    {
        public override void register(DBServer server)
        {
            base.register(server);

            server.dispatcher.addHandler(new DBStart());
            server.dispatcher.addHandler(new DBQuery());
            server.dispatcher.addHandler(new DBTest());
            server.dispatcher.addHandler(new DBGetSummary());
        }
    }
}