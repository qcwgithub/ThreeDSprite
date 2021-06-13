using Data;
using System.Collections.Generic;
using System;

namespace Script
{
    public class JsonMessagePackerS : JsonMessagePacker, IServerScript<Server>
    {
        public Server server { get; set; }

        
        protected override JsonUtils JSON 
        {
            get
            {
                return this.server.JSON;
            }
        }
        protected override Dictionary<string, Type> name2Type
        {
            get
            {
                return this.server.dataEntry.name2Type;
            }
        }
        protected override void onError(string msg)
        {
            this.server.logger.Error(msg);
        }
    }
}