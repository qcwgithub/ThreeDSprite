public abstract class DBHandler : Handler {
    public DBData dbData { get { return this.server.dbData; } }
    public DBScript dbScript { get { return this.server.dbScript; } }
}