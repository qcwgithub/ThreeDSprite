// using System.Threading.Tasks;
// using Data;

// namespace Script
// {
//     public class LocStart : LocHandler
//     {
//         public override MsgType msgType { get { return MsgType.Start; } }

//         public override Task<MyResponse> handle(TcpClientData socket, string msg/* no use */)
//         {
//             this.baseScript.setState(ServerState.Starting);

//             this.server.tcpListenerScript.listen(() => false);

//             this.baseScript.setState(ServerState.Started);
//             return ECode.Success.toTask();
//         }
//     }
// }