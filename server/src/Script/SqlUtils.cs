
public class SqlUtils : IScript {
    public Server server { get; set; }
    public baseScript { get { return this.server.baseScript; } }
}