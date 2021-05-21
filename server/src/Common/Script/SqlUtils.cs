
public class SqlUtils : IScript {
    public Server server { get; set; }
    public BaseScript baseScript { get { return this.server.baseScript; } }
}