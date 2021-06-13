using System.Collections.Generic;
using System.Linq;
using Data;

namespace Script
{
    public class SqlUtils<T> : IServerScript<T> where T : Server
    {
        public T server { get; set; }
    }
}