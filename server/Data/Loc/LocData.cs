using System.Collections.Generic;

namespace Data
{
    public sealed class LocData : ServerBaseData
    {
        public Dictionary<int, LocServerInfo> map = new Dictionary<int, LocServerInfo>();
    }
}