using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

public class BaseScript : IScript
{
    public Server server { get; set; }

    public BaseData baseData { get { return this.server.baseData; } }
    public log4net.ILog logger { get { return this.server.logger; } }
    public MessageDispatcher dispatcher { get { return this.server.dispatcher; } }

    public void addKnownLoc(Loc loc)
    {
        this.baseData.knownLocs[loc.id] = loc;
    }
    public Loc myLoc()
    {
        return this.baseData.knownLocs[this.baseData.id];
    }
    public string getKnownUrlForServer(int id)
    {
        Loc loc;
        if (!this.baseData.knownLocs.TryGetValue(id, out loc))
        {
            Console.WriteLine("loc == null, id: " + id);
            // process.exit(1);
        }
        return this.server.serverNetwork.urlForServer(loc.inIp, loc.port);
    }
    public Loc getKnownLoc(int id)
    {
        Loc loc;
        if (!this.baseData.knownLocs.TryGetValue(id, out loc))
        {
            return loc;
        }
        return loc;
    }

    public bool isLocalhost()
    {
        return this.myLoc().inIp == "localhost";
    }

    public bool isDevelopment()
    {
        // return process.env.NODE_ENV == "development";
        return true;
    }

    // private readonly _GeneratorFunction = Object.getPrototypeOf(function*(){}).constructor;
    // isGeneratorFunction(object t): boolean {
    //     return t && t.constructor == this._GeneratorFunction;
    // }
    // trackConnectStatus(s: SocketIOClient.Socket, string meString, string targetString) {
    //     var string events[] = [
    //         "connect",
    //         "connect_error",
    //         "connect_timeout",
    //         "connecting",
    //         "disconnect",
    //         "error",
    //         "reconnect",
    //         "reconnect_attempt",
    //         "reconnect_failed",
    //         "reconnect_error",
    //         "reconnecting",
    //         "ping",
    //         "pong"
    //     ];

    //     for (int i = 0; i < events.length; i++) {
    //         var e = events[i];
    //         s.on(e, () => {
    //             console.log(meString + " -> " + targetString + " status: " + e);
    //         });
    //     }
    // }

    public bool isNumber(string s)
    {
        int v;
        return int.TryParse(s, out v);
    }

    public bool isBoolean(object v)
    {
        return v is bool;
    }

    // ids=null 表示全部，monitor使用
    public async Task<MyResponse> requestLocationAsync(int[] ids)
    {
        this.logger.Info("requstLoc " + this.server.JSON.stringify(ids));
        while (true)
        {
            var r = await this.baseData.locSocket.sendAsync(
                MsgType.LocRequestLoc,
                new MsgLocRequestLoc { ids = new List<int>(ids) });

            if (r.err != ECode.Success)
            {
                await this.waitAsync(1000);
            }
            else
            {
                var res = this.server.JSON.parse<ResLocRequestLoc>(r.res as string);
                List<Loc> locs = res.locs;
                for (int i = 0; i < locs.Count; i++)
                {
                    this.addKnownLoc(locs[i]);
                }
                break;
            }
        }
        this.logger.Info("requstLocation OK");
        return ECode.Success;
    }

    public async Task<ISocket> connectAsync(int toId)
    {
        string url = this.getKnownUrlForServer(toId);
        this.logger.Info("connectYield " + url);
        string to = Utils.numberId2stringId(toId);

        string msgOnConnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });
        string msgOnDisconnect = this.server.JSON.stringify(new MsgOnConnect { isListen = false, isServer = true });

        ISocket s = await this.server.serverNetwork.connectAsync(url,
            (ISocket socket) =>
            { // onconnect
                //this.logger.info("-> %s: connected", to);
                this.dispatcher.dispatch(socket, MsgType.OnConnect, msgOnConnect, null);
            },
            (ISocket socket) =>
            { // onDisconnect
                //this.error("-> %s: disconnected", to);
                this.dispatcher.dispatch(socket, MsgType.OnDisconnect, msgOnDisconnect, null);
            });

        return s;
    }

    public void listen(Func<bool> acceptClient)
    {
        string msgOnConnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });
        string msgOnDisconnect_true = this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = true });

        string msgOnConnect_false= this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });
        string msgOnDisconnect_false= this.server.JSON.stringify(new MsgOnConnect { isListen = true, isServer = false });

        int port = this.myLoc().port;
        this.server.serverNetwork.listen(port, acceptClient, (ISocket socket, bool isServer) =>
        {
            this.dispatcher.dispatch(socket, MsgType.OnConnect, isServer ? msgOnConnect_true : msgOnConnect_false, null);
        }, (ISocket socket, bool isServer) =>
        {
            this.dispatcher.dispatch(socket, MsgType.OnDisconnect, isServer ? msgOnDisconnect_true : msgOnDisconnect_false, null);
        });
    }
    
    public T decodeMsg<T>(string msg)
    {
        return this.server.JSON.parse<T>(msg);
    }
    // public queryDbAccountYield(string queryStr): any {
    //     return new RequestObject(this.server, this.baseData.dbAccountSocket, MsgType.DBQuery, { queryStr: queryStr });
    // }
    // public queryDbPlayerYield(string queryStr): any {
    //     return new RequestObject(this.server, this.baseData.dbPlayerSocket, MsgType.DBQuery, { queryStr: queryStr });
    // }
    // public queryDbLogYield(string queryStr): any {
    //     return new RequestObject(this.server, this.baseData.dbLogSocket, MsgType.DBQuery, { queryStr: queryStr });
    // }
    // public queryDbLog(string queryStr) {
    //     this.server.netProto.send(this.baseData.dbLogSocket, MsgType.DBQuery, { queryStr: queryStr }, null);
    // }
    public Task waitAsync(int timeoutMs)
    {
        return Task.Delay(timeoutMs);
    }

    // TODO
    // 发送给一个非*消息会卡住
    public Task<MyResponse> sendToSelfYield(MsgType type, object msg)
    {
        var waiter = new WaitCallBack();
        this.dispatcher.dispatch(null, type, this.server.JSON.stringify(msg), (e, r) => waiter.finish(e, r));
        return waiter.Task;
    }
    public void sendToSelf(MsgType type, object msg)
    {
        this.dispatcher.dispatch(null, type, this.server.JSON.stringify(msg), null);
    }

    ///////// timer ///////////
    public int setTimerOnce(int timeMs, MsgType type, object msg, Action<ECode, string> reply = null)
    {
        string msgStr = this.server.JSON.stringify(msg);
        return this.server.timerScript.setTimer(() =>
        {
            this.dispatcher.dispatch(null, type, msgStr, reply);
        }, timeMs);
    }

    public int setTimerLoop(int timeMs, MsgType type, object msg, Action<ECode, string> reply = null)
    {
        string msgStr = this.server.JSON.stringify(msg);
        return this.server.timerScript.setInterval(() =>
        {
            this.dispatcher.dispatch(null, type, msgStr, reply);
        }, timeMs);
    }

    ///////// lock ///////////
    public void @lock(string s)
    {
        this.baseData.locks.Add(s);
    }
    public bool isLocked(string s)
    {
        return this.baseData.locks.Contains(s);
    }
    public void unlock(string s)
    {
        this.baseData.locks.Remove(s);
    }

    public void setState(ServerState s)
    {
        
        this.baseData.state = s;
        if (this.logger != null)
        {
            this.logger.Info(s);
        }
        else
        {
            Console.WriteLine(s);
        }
        if (s == ServerState.ReadyToShutdown)
        {
            bool canExit = true;
            for (int i = 0; i < this.baseData.allServers.Count; i++)
            {
                if (this.baseData.allServers[i].baseData.id == ServerConst.MONITOR_ID)
                {
                    // 忽略 Monitor
                    continue;
                }
                if (this.baseData.allServers[i].baseData.state != ServerState.ReadyToShutdown)
                {
                    canExit = false;
                    break;
                }
            }
            if (canExit)
            {
                // process.exit(0);
            }
        }
    }

    public T loadJson<T>(string file)
    {
        string str = File.ReadAllText(file, Encoding.UTF8);
        T obj = this.server.JSON.parse<T>(str);
        return obj;
    }

    // AAA 服务器没有 GameScript
    DateTime baseDate = new DateTime(1970, 1, 1);
    public int getTimeMs()
    {
        return (int)(DateTime.Now - baseDate).TotalMilliseconds;
    }
}