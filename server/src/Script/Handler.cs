
public abstract class Handler : IScript {
    public Server server { get; set; }
    public BaseData baseData { get { return this.server.baseData; } }
    public BaseScript baseScript { get { return this.server.baseScript; } }
    public object logger { get { return this.server.logger; } }
    public MessageDispatcher dispatcher { get { return this.server.dispatcher; } }

    public abstract MsgType msgType { get; }
    public abstract MyResponse handle(object socket, object msg);
    public virtual void postHandle(object socket, object msg) { }
    public string msgName {
        get {
            return this.msgType.ToString();
        }
    }
}