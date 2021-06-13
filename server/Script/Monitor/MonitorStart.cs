// using System;
// using System.IO;
// using System.Threading;
// using System.Threading.Tasks;
// using Data;

// namespace Script
// {
//     public class MonitorStart : MonitorHandler
//     {
//         public override MsgType msgType => MsgType.Start;

//         public override async Task<MyResponse> handle(TcpClientData socket, object _msg)
//         {
//             this.baseScript.setState(ServerState.Starting);

//             // connect to loc
//             this.baseData.locSocket = await this.baseScript.connectAsync(ServerConst.LOC_ID);
//             this.server.timerScript.setTimer(1000, MsgType.KeepAliveToLoc, null, true);

//             // monitor file
//             // this.server.dispatcher.dispatch(null, MsgType.MonitorWatchFile, null, null);

//             // start watch file
//             this.server.monitorData.inputFileName = @".\input\input.txt";
//             var watcher = this.server.monitorData.watcher = new FileSystemWatcher(@".\input");
//             watcher.Filter = "input.txt";
//             watcher.NotifyFilter = NotifyFilters.LastWrite;
//             watcher.IncludeSubdirectories = false;
//             watcher.EnableRaisingEvents = true;
//             watcher.Changed += this.server.monitorData.OnChanged;

//             this.baseScript.setState(ServerState.Started);
//             return ECode.Success;
//         }
//     }
// }