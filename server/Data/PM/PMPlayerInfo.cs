using System.Collections.Generic;

// 一个玩家

namespace Data
{
    // 纯数据，无逻辑
    public sealed class PMPlayerInfo : IProfileInput
    {
        public CProfile profile { get; set; }

        public TcpClientData socket = null;
        public string channel = null;
        public string channelUserId = null;
        public string token = null;
        public int id = 0;
        public int destroyTimer = -1;

        public int onlineTimeMs = 0;

        public bool iOSPaying = false;
        public bool ltPaying = false;
        public bool ivyPaying = false;

        public int saveTimer = -1;

        //// 1 ////
        public List<int> dataChanged = new List<int>();
        public List<int> dataLast = new List<int>();

        //// 2 ////
        public SqlTablePlayer lastProfile = null;
    }
}