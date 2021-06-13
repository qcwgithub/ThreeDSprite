using System;
using System.Net.Sockets;
using System.IO;

namespace Data
{
    public class TimerScriptProxy
    {
        public Action<TimerData> onTimerTick;
    }
}