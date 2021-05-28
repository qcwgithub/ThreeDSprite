using Data;

namespace Script
{
    public interface IScript<T> where T : Server
    {
        T server { get; }
    }
}