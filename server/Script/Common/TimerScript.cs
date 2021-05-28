using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class TimerData
    {
        public TimerScript parent;
        public int id;
        public Action action;
        public int timeoutMs;
        public bool loop;
        public bool cancelled = false;

        public async void Start()
        {
            while (true)
            {
                await Task.Delay(this.timeoutMs);
                if (this.cancelled)
                {
                    break;
                }
                this.action();
                if (!this.loop)
                {
                    break;
                }
            }
            this.parent.clearTimer(this.id);
        }
    }

    // 临时方案
    public class TimerScript : IServerScript<Server>
    {
        public Server server { get; set; }

        private int nextId = 1;
        public Dictionary<int, TimerData> Dict = new Dictionary<int, TimerData>();

        public int setTimer(Action action, int timeoutMs)
        {
            var timer = new TimerData { parent = this, id = this.nextId++, action = action, timeoutMs = timeoutMs, loop = false };
            this.Dict.Add(timer.id, timer);
            timer.Start();
            return timer.id;
        }

        public void clearTimer(int timer)
        {
            TimerData timerData;
            if (this.Dict.TryGetValue(timer, out timerData))
            {
                timerData.cancelled = true;
            }
        }

        public int setInterval(Action action, int timeoutMs)
        {
            var timer = new TimerData { parent = this, id = this.nextId++, action = action, timeoutMs = timeoutMs, loop = true };
            this.Dict.Add(timer.id, timer);
            timer.Start();
            return timer.id;
        }

        public void clearInterval(int timer)
        {
            TimerData timerData;
            if (this.Dict.TryGetValue(timer, out timerData))
            {
                timerData.cancelled = true;
            }
        }
    }
}