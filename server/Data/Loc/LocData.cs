using System.Collections.Generic;

namespace Data
{
    public sealed class LocData : ServerData
    {
        public Dictionary<int, LocServerInfo> map = new Dictionary<int, LocServerInfo>();
    }
}