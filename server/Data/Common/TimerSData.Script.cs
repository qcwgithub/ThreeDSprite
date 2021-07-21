using System;
using System.Collections.Generic;

namespace Data
{
    public partial class TimerSData
    {
        void onTick()
        {
            if (this.triggerDict.Count == 0)
            {
                return;
            }

            var nowS = this.getTimeS();
            if (nowS < this.minTimeS)
            {
                return;
            }

            List<TimerInfo> list = this.triggerDict[this.minTimeS];
            this.triggerDict.Remove(this.minTimeS);

            foreach (TimerInfo info in list)
            {
                this.serverData.tcpClientCallback.dispatch(null, info.msgType, info.msg, null);
            }

            // reset minTimeS to a big value
            this.minTimeS = int.MaxValue;

            // update minTimeS
            foreach (var kv in this.triggerDict)
            {
                this.minTimeS = kv.Key;
                break;
            }

            foreach (TimerInfo info in list)
            {
                if (!info.loop)
                {
                    this.clearTimer(info.timerId);
                }
                else
                {
                    info.nextTimeS = nowS + info.timeoutS;
                    this.addTrigger(info);
                }
            }

        }

        public int getTimeS()
        {
            return (int)(DateTime.Now - this.baseDate).TotalSeconds;
        }

        private void addTrigger(TimerInfo info)
        {
            if (info.nextTimeS < this.minTimeS)
            {
                this.minTimeS = info.nextTimeS;
            }

            List<TimerInfo> list;
            if (!this.triggerDict.TryGetValue(info.nextTimeS, out list))
            {
                list = new List<TimerInfo>();
                this.triggerDict.Add(info.nextTimeS, list);
            }
            list.Add(info);
        }

        public int setTimer(int timeoutS, MsgType msgType, object msg/*, bool loop*/)
        {
            bool loop = false;
            if (timeoutS < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (timeoutS == 0 && loop)
            {
                throw new InvalidOperationException();
            }

            var info = new TimerInfo
            {
                timerId = this.nextId++,
                timeoutS = timeoutS,
                nextTimeS = this.getTimeS() + timeoutS,
                msgType = msgType,
                msg = msg,
                loop = loop
            };
            this.timerDict.Add(info.timerId, info);

            this.addTrigger(info);

            return info.timerId;
        }

        public void clearTimer(int timerId)
        {
            TimerInfo info;
            if (this.timerDict.TryGetValue(timerId, out info))
            {
                this.timerDict.Remove(info.timerId);

                List<TimerInfo> list;
                if (this.triggerDict.TryGetValue(info.nextTimeS, out list))
                {
                    int index = list.FindIndex(ele => ele.timerId == timerId);
                    if (index >= 0)
                    {
                        list.RemoveAt(index);
                    }
                }
            }
        }
    }
}