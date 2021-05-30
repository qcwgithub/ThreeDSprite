using System.Threading.Tasks;
namespace Data
{
    public interface IHandler
    {
        MsgType msgType { get; }
        Task<MyResponse> handle(TcpClientData socket, string _msg);
        void postHandle(object socket, object msg);
    }
}