using System;
using System.Linq;
using System.Net.Sockets;
using Data;
using System.Threading.Tasks;

namespace Script
{
    // Server 提供给 IScript 数据、其他脚本的访问
    public abstract partial class Server
    {
        public void addKnownLoc(Loc loc)
        {
            this.data.knownLocs[loc.id] = loc;
        }
        public Loc myLoc()
        {
            return this.data.knownLocs[this.data.id];
        }
        public Loc getKnownLoc(int id)
        {
            Loc loc;
            return this.data.knownLocs.TryGetValue(id, out loc) ? loc : null;
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

        public T CastObject<T>(object msg)
        {
            if (msg == null || !(msg is T))
            {
                throw new InvalidCastException();
            }
            return (T)msg;
        }

        public Task waitAsync(int timeoutMs)
        {
            return Task.Delay(timeoutMs);
        }

        public void setState(ServerState s)
        {
            this.data.state = s;
            this.logger.Info(s);
            if (s == ServerState.ReadyToShutdown)
            {
                bool canExit = true;

                foreach (var kv in this.dataEntry.serverDatas)
                {
                    if (kv.Key == ServerConst.MONITOR_ID)
                    {
                        // 忽略 Monitor
                        continue;
                    }
                    if (kv.Value == null)
                    {
                        // 有时候服务器没创建，我跳过了，这里先兼容一下没关系
                        continue;
                    }
                    if (kv.Value.state != ServerState.ReadyToShutdown)
                    {
                        canExit = false;
                        break;
                    }
                }
                if (canExit)
                {
                    this.logger.Info("Exit now...");
                    Console.WriteLine("{0} exit now...", Utils.numberId2stringId(this.id));
                    Environment.Exit(0);
                }
            }
        }

        // AAA 服务器没有 GameScript
        DateTime baseDate = new DateTime(1970, 1, 1);
        public int getTimeMs()
        {
            return (int)(DateTime.Now - baseDate).TotalMilliseconds;
        }
        public int getTimeS()
        {
            return (int)(DateTime.Now - baseDate).TotalSeconds;
        }

        public DateTime SecondsToDateTime(int seconds)
        {
            return baseDate.AddSeconds(seconds);
        }
        public int DateTimeToSeconds(DateTime dt)
        {
            return (int)(dt - baseDate).TotalSeconds;
        }
    }
}