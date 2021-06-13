namespace Script
{
    public interface IGameScripts
    {
        BattlefieldScript BattlefieldScript { get; }
        SCUtils scUtils { get; }
        JsonUtils JSON { get; }
    }
}