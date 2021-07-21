using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

namespace Data
{
    public class TimerInfo
    {
        public int timerId;
        public int timeoutS;
        public int nextTimeS;
        public MsgType msgType;
        public object msg;
        public bool loop;
    }

    public class TimerDataComparer : IComparer<int>
    {
        public int Compare(int s1, int s2)
        {
            return s1 - s2;
        }
    }

    public partial class TimerSData
    {
        public int nextId = 1;
        public ServerData serverData;

        public SortedDictionary<int, List<TimerInfo>> triggerDict = new SortedDictionary<int, List<TimerInfo>>(new TimerDataComparer());
        public Dictionary<int, TimerInfo> timerDict = new Dictionary<int, TimerInfo>();

        public DateTime baseDate = new DateTime(1970, 1, 1);
        public int minTimeS = int.MaxValue;

        public bool started { get; private set; }
        public int tickIntervalMs = 1000;
        public async void start()
        {
            if (started) return;
            started = true;

            while (true)
            {
                await Task.Delay(tickIntervalMs);
                this.onTick();
            }
        }
    }
}