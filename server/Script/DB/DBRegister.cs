public class DBRegister : BaseRegister {
    public override void register(Server server) {
        base.register(server);

        server.dispatcher.addHandler(new DBStart());
        server.dispatcher.addHandler(new DBQuery());
        server.dispatcher.addHandler(new DBTest());
        server.dispatcher.addHandler(new DBGetSummary());
    }
}