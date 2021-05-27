public class DBScript : IScript {
    private Server _server;
    public Server server
    {
        get { return this.server; }
        set { this._server = value; }
    }
}