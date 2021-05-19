public class BaseRegister{
    public virtual void register(Server server) {
        server.dispatcher.addHandler(new OnShutdown());
        server.dispatcher.addHandler(new OnReloadScript());
        server.dispatcher.addHandler(new OnRunScript());
        server.dispatcher.addHandler(new OnConnect());
        server.dispatcher.addHandler(new OnDisconnect());
        server.dispatcher.addHandler(new KeepAliveToLoc());
    }
}