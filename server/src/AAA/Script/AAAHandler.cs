
public abstract class AAAHandler : Handler {
    public AAAData aaaData { get { return this.server.aaaData; } }
    public AAAScript aaaScript { get { return this.server.aaaScript; } }
}