using Data;

namespace Script
{
    public interface IServerScript<T> where T : Server
    {
        T server { get; }
    }
}