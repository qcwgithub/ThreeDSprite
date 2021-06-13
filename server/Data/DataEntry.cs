using System;
using System.Collections.Generic;

namespace Data
{
    // 所有数据的入口
    public class DataEntry
    {
        public bool inited = false;
        public ProcessData processData;
        public List<int> serverIds;
        public Purpose purpose;
        public int timezoneOffset;
        public string androidVersion;
        public string iOSVersion;
        public ServerConst serverConst;

        public ThisMachineConfig thisMachineConfig;
        public Loc locLoc;

        public Dictionary<int, ServerData> serverDatas;
        public Dictionary<string, Type> name2Type;
    }
}
