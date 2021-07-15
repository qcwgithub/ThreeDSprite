using System;
using System.Linq;
using System.Net.Sockets;
using Data;

namespace Script
{
    // Server 提供给 IScript 数据、其他脚本的访问
    public abstract partial class Server
    {
        public int id;
        public int scriptDllVersion;
        public DataEntry dataEntry;
        public ServerData data { get; private set; }

        public log4net.ILog logger { get; private set; }
        public TcpListenerScript tcpListenerScript;
        public TcpClientScriptS tcpClientScript;

        protected MessageDispatcher dispatcher;
        public SqlLog sqlLog;
        public Utils utils;
        public JsonUtils JSON { get; private set; }
        public IMessagePacker messagePacker;

        protected void AddHandler<T>() where T : Server
        {
            this.dispatcher.addHandler(new AskForStart() { server = this });
            this.dispatcher.addHandler(new OnStart() { server = this });
            this.dispatcher.addHandler(new OnShutdown<T> { server = (T)this });
            this.dispatcher.addHandler(new OnSocketConnect<T> { server = (T)this });
            this.dispatcher.addHandler(new OnSocketClose<T> { server = (T)this });
            this.dispatcher.addHandler(new KeepAliveToLoc<T> { server = (T)this });
            this.dispatcher.addHandler(new OnReloadScript() { server = this });
            this.dispatcher.addHandler(new KeepServerConnections() { server = this });
        }

        public virtual void OnLoad(DataEntry dataEntry, int id, int scriptDllVersion)
        {
            this.id = id;
            this.scriptDllVersion = scriptDllVersion;
            this.dataEntry = dataEntry;
            this.data = this.dataEntry.serverDatas[this.id];
            this.logger = this.data.logger;

            Console.WriteLine("**** {0}.OnLoad, V{1}", Utils.numberId2stringId(this.id), this.scriptDllVersion);

            this.tcpListenerScript = new TcpListenerScript { server = this };
            this.tcpClientScript = new TcpClientScriptS { server = this };

            this.dispatcher = new MessageDispatcher { server = this };
            this.utils = new Utils();
            this.JSON = new JsonUtils();
            // this.messagePacker = new JsonMessagePackerS { server = this };
            this.messagePacker = new BinaryMessagePacker();
            this.sqlLog = new SqlLog { server = this };


            // script proxy
            this.data.scriptProxy = new TcpListenerScriptProxy
            {
                onListenerComplete = (listener, e) => listener.onComplete(e),
                onAcceptComplete = (listener, e) => this.tcpListenerScript.onAcceptComplete(listener, e),
            };

            this.data.timerScriptProxy = new TimerScriptProxy
            {
                onTimerTick = (timerData) => timerData.onTick(),
            };

            this.data.tcpClientCallback = this.tcpClientScript;
        }

        public virtual void OnStart()
        {
            if (!data.timerSData.started)
            {
                data.timerSData.start();
            }

            if (data.tcpListenerForServer != null)
            {
                data.tcpListenerForServer.listen(data.knownLocs[data.id].inPort);
                data.tcpListenerForServer.accept();
            }

            if (data.tcpListenerForClient != null)
            {
                data.tcpListenerForClient.listen(data.knownLocs[data.id].outPort);
                data.tcpListenerForClient.accept();
            }

            if (data.connectToServerIds.Contains(ServerConst.LOC_ID))
            {
                this.setTimer(0, MsgType.KeepAliveToLoc, null);
            }
            if (data.connectToServerIds.Count > 0)
            {
                this.setTimer(0, MsgType.KeepServerConnections, null);
            }
        }

        public virtual void OnUnload()
        {
            Console.WriteLine("**** {0}.OnUnload, V{1}", Utils.numberId2stringId(this.id), this.scriptDllVersion);

            // 这个不需要unload，等着被换掉
            // this.baseData.scriptProxy = null;
        }

        public void proxyDispatch(TcpClientData data, MsgType msgType, object msg, Action<ECode, object> reply)
        {
            this.data.tcpClientCallback.dispatch(data, msgType, msg, reply);
        }

        public void rawDispatch(TcpClientData data, MsgType msgType, object msg, Action<ECode, object> reply)
        {
            this.dispatcher.dispatch(data, msgType, msg, reply);
        }
    }
}
