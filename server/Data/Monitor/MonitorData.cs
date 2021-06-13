using System.IO;

namespace Data
{
    public sealed class MonitorData : ServerData
    {
        public string inputFileName;
        public FileSystemWatcher watcher;
        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                this.tcpClientScriptProxy.dispatch(null, MsgType.MonitorOnInput, null, null);
            }
        }
    }
}