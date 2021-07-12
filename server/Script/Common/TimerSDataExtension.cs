using Data;
using System;
using System.Collections.Generic;

namespace Script
{
    public static class TimerSDataExtension
    {
        public static void onTick(this TimerSData @this)
        {
            if (@this.triggerMap.Count == 0)
            {
                return;
            }

            var nowS = @this.getTimeS();
            if (nowS < @this.minTimeS)
            {
                return;
            }

            List<TimerInfo> list = @this.triggerMap[@this.minTimeS];
            @this.triggerMap.Remove(@this.minTimeS);

            foreach (TimerInfo info in list)
            {
                @this.serverData.tcpClientScriptProxy.dispatch(null, info.msgType, info.msg, null);
            }

            // reset minTimeS to a big value
            @this.minTimeS = int.MaxValue;

            // update minTimeS
            foreach (var kv in @this.triggerMap)
            {
                @this.minTimeS = kv.Key;
                break;
            }

            foreach (TimerInfo info in list)
            {
                if (!info.loop)
                {
                    @this.clearTimer(info.id);
                }
                else
                {
                    info.nextTimeS = nowS + info.timeoutS;
                    @this.addTrigger(info);
                }
            }

        }

        public static int getTimeS(this TimerSData @this)
        {
            return (int)(DateTime.Now - @this.baseDate).TotalSeconds;
        }

        private static void addTrigger(this TimerSData @this, TimerInfo info)
        {
            if (info.nextTimeS < @this.minTimeS)
            {
                @this.minTimeS = info.nextTimeS;
            }

            List<TimerInfo> list;
            if (!@this.triggerMap.TryGetValue(info.nextTimeS, out list))
            {
                list = new List<TimerInfo>();
                @this.triggerMap.Add(info.nextTimeS, list);
            }
            list.Add(info);
        }

        public static int setTimer(this TimerSData @this, int timeoutS, MsgType msgType, object msg/*, bool loop*/)
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
                id = @this.nextId++,
                timeoutS = timeoutS,
                nextTimeS = @this.getTimeS() + timeoutS,
                msgType = msgType,
                msg = msg,
                loop = loop
            };
            @this.timerMap.Add(info.id, info);

            @this.addTrigger(info);

            return info.id;
        }

        public static void clearTimer(this TimerSData @this, int id)
        {
            TimerInfo info;
            if (@this.timerMap.TryGetValue(id, out info))
            {
                @this.timerMap.Remove(info.id);

                List<TimerInfo> list;
                if (@this.triggerMap.TryGetValue(info.nextTimeS, out list))
                {
                    int index = list.FindIndex(ele => ele.id == id);
                    if (index >= 0)
                    {
                        list.RemoveAt(index);
                    }
                }
            }
        }
    }
}