using System.Net.Sockets;
using Data;

namespace Script
{
    // Data 与 Script 交换数据
    public class Communication
    {
        public GlobalData globalData;
        public Socket listenSocket;
    }
}