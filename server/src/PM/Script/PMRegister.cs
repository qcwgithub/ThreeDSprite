// import PMUploadProfile from "./PMUploadProfile";

public class PMRegister : BaseRegister {
    public override void register(Server server) {
        base.register(server);

        server.dispatcher.addHandler(new PMStart());
        server.dispatcher.addHandler(new PMOnDisconnect());
        server.dispatcher.addHandler(new PMKeepAliveToAAA());
        server.dispatcher.addHandler(new PMPlayerLogin());
        server.dispatcher.addHandler(new PMChangeChannel());
        server.dispatcher.addHandler(new PMPreparePlayerLogin());
        server.dispatcher.addHandler(new PMDestroyPlayer());
        // server.dispatcher.addHandler(new PMUploadProfile());

        server.dispatcher.addHandler(new PMAction());
    }
}