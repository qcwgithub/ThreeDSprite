using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Data;
using log4net;
using log4net.Config;
using log4net.Repository;
using System.Linq;

namespace Script
{
    public class ScriptEntry : IScriptEntry
    {
        Dictionary<string, string> ParseArguments(string[] args)
        {
            var argMap = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                int index = arg.IndexOf('=');
                if (index > 0)
                {
                    string key = arg.Substring(0, index);
                    string value = arg.Substring(index + 1);
                    Console.WriteLine(key + ": " + value);
                    argMap.Add(key, value);
                }
            }
            return argMap;
        }

        bool InitDataOnce(string[] args, GlobalData global, JsonUtils JSON)
        {
            var argMap = this.ParseArguments(args);

            List<int> serverIds = null;
            string ids = argMap["ids"];
            if (ids == "all")
                serverIds = new List<int> { 1, 2, 3, 11, 12, 13, 101 };
            else
                serverIds = JSON.parse<List<int>>(ids);

            if (serverIds == null || serverIds.Count == 0)
            {
                Console.WriteLine("serverIds.Count == 0");
                return false;
            }

            var purpose = Enum.Parse<Purpose>(argMap["purpose"]);

            var configLoader = new ConfigLoader();
            configLoader.Load(JSON, purpose);

            var log4NetCreation = new Log4netCreation();
            log4NetCreation.Create(serverIds.Select(_ => Utils.numberId2stringId(_)).ToList(), configLoader);

            var dataCreation = new DataCreation();
            dataCreation.Create(global, configLoader, serverIds, purpose, log4NetCreation);

            return true;
        }

        public bool OnLoad(string[] args, GlobalData global)
        {
            JsonUtils JSON = new JsonUtils();
            if (!global.inited)
            {
                if (!this.InitDataOnce(args, global, JSON))
                {
                    return false;
                }

                global.inited = true;

                //------------------------
                // 异步方法全部会回掉到主线程
                SynchronizationContext.SetSynchronizationContext(ET.ThreadSynchronizationContext.Instance);
            }

            var serverCreation = new ServerCreation();
            List<Server> servers = serverCreation.Create(global);
            // foreach (Server server in servers)
            // {
            //     server.dispatcher.dispatch(null, MsgType.Start, null, null);
            // }

            return true;
        }

        public void OnUnload()
        {
            
        }
    }
}
