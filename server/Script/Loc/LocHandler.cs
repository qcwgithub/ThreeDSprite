using Data;
public abstract class LocHandler : Handler
{
    public LocData locData { get { return this.server.locData; } }
    public LocScript locScript { get { return this.server.locScript; } }
}