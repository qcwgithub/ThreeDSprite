using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Data;
using log4net;
using log4net.Config;
using log4net.Repository;
using System.Linq;
using MessagePack.Resolvers;
using MessagePack;

namespace Script
{
    public class ScriptEntry : IScriptEntry
    {
        bool InitDataOnce(Dictionary<string, string> args, DataEntry global, JsonUtils JSON)
        {
            List<int> serverIds = null;
            string ids = args["ids"];
            if (ids == "all")
                serverIds = new List<int> { 1, 2, 3, 4, 5, 11, 12, 13, 101, 201 };
            else
                serverIds = JSON.parse<List<int>>(ids);

            if (serverIds == null || serverIds.Count == 0)
            {
                Console.WriteLine("serverIds.Count == 0");
                return false;
            }

            var purpose = Enum.Parse<Purpose>(args["purpose"]);

            var configLoader = new ConfigLoader();
            configLoader.Load(JSON, purpose);

            var log4NetCreation = new Log4netCreation();
            log4NetCreation.Create(serverIds.Select(_ => Utils.numberId2stringId(_)).ToList(), configLoader);

            var dataCreation = new DataCreation();
            dataCreation.Create(global, configLoader, serverIds, purpose, log4NetCreation);

            return true;
        }

        List<Server> servers;
        BMServer bmServer;
        public bool needUpdate
        {
            get
            {
                return this.bmServer != null;
            }
        }
        int version;
        public int GetVersion()
        {
            return this.version;
        }



        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static bool serializerRegistered = false;
            // init message pack
        static void Initialize()
        {
            if (!serializerRegistered)
            {
                StaticCompositeResolver.Instance.Register(
                     MessagePack.Resolvers.GeneratedResolver.Instance,
                     MessagePack.Unity.UnityResolver.Instance,
                     MessagePack.Resolvers.StandardResolver.Instance
                );

                var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

                MessagePackSerializer.DefaultOptions = option;
                serializerRegistered = true;
            }
        }

        public void Update(float dt)
        {
            this.bmServer.Update(dt);
        }

        public bool OnLoad(Dictionary<string, string> args, DataEntry dataEntry, int version)
        {
            // init message pack
            Initialize();

            this.version = version;
            JsonUtils JSON = new JsonUtils();
            if (!dataEntry.inited)
            {
                if (!this.InitDataOnce(args, dataEntry, JSON))
                {
                    return false;
                }

                dataEntry.inited = true;
            }

            var serverCreation = new ServerCreation();
            this.servers = serverCreation.Create(dataEntry, version);
            foreach (Server server in this.servers)
            {
                server.proxyDispatch(null, MsgType.AskForStart, null, null);
                if (server is BMServer)
                {
                    this.bmServer = (BMServer) server;
                }
            }

            return true;
        }

        public void OnUnload()
        {
            foreach (Server server in this.servers)
            {
                server.OnUnload();
            }
        }
    }
}
