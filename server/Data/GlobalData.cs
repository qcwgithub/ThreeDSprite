using System;
using System.Collections.Generic;

namespace Data
{
    public class GlobalData
    {
        public bool inited = false;

        public ProcessData processData;
        public List<int> serverIds;
        public Purpose purpose;
        public int timezoneOffset;
        public string androidVersion;
        public string iOSVersion;

        public Dictionary<int, ServerData> serverDatas;
    }
}
