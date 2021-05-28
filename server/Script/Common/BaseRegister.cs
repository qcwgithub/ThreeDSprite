namespace Script
{
    public class BaseRegister<T> where T: Server
    {
        public virtual void register(T server)
        {
            server.dispatcher.addHandler(new OnShutdown<T>());
            // server.dispatcher.addHandler(new OnReloadScript());
            // server.dispatcher.addHandler(new OnRunScript());
            server.dispatcher.addHandler(new OnConnect<T>());
            server.dispatcher.addHandler(new OnDisconnect<T>());
            server.dispatcher.addHandler(new KeepAliveToLoc<T>());
        }
    }
}