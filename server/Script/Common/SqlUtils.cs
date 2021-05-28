using System.Collections.Generic;
using System.Linq;
using Data;

namespace Script
{
    public class SqlUtils<T> : IScript<T> where T : Server
    {
        public T server { get; set; }
        public BaseScript baseScript { get { return this.server.baseScript; } }

        public Dictionary<string, List<object>> DecodeSqlRecords(object res)
        {
            var json = res as string;
            var dict = this.server.JSON.parse<Dictionary<string, List<object>>>(json);
            return dict;
        }
        public int GetRecordsCount(Dictionary<string, List<object>> dict)
        {
            if (dict == null || dict.Count == 0)
            {
                return 0;
            }
            return dict.First().Value.Count;
        }
    }
}