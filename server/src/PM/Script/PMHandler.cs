
public abstract class PMHandler : Handler {
    public PMData pmData { get { return this.server.pmData; } }
    public PMScript pmScript { get { return this.server.pmScript; } }
    public PMSqlUtils pmSqlUtils { get { return this.server.pmSqlUtils; } }
    public SqlLog sqlLog { get { return this.server.sqlLog; } }
}