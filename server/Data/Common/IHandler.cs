using System.Threading.Tasks;
namespace Data
{
    public interface IHandler
    {
        Task<MyResponse> handle(ISocket socket, string _msg);
        void postHandle(object socket, object msg);
    }
}