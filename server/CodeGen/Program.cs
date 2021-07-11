using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace CodeGen
{
    public class Program
    {
        static List<string> MatchMpc(string file)
        {
            List<string> ret = new List<string>();
            // var allDataTypes = typeof(MsgLoginAAA).Assembly.GetTypes();
            string clientMpcText = File.ReadAllText(file);
            //{ typeof(global::Data.BMBattleInfo), 4 },
            Regex reg = new Regex(@"{ typeof\(global\:\:Data\.(\w+)\)\, \d+ }\,");
            Match match = reg.Match(clientMpcText);
            while (match.Success)
            {
                ret.Add(match.Groups[1].ToString());
                match = match.NextMatch();
            }
            return ret;
        }

        public static void Main(string[] args)
        {
            List<string> client = MatchMpc(args[0]);
            for (int i = 0; i < client.Count; i++)
            {
                Console.WriteLine("[Client] {0} of {1} {2}", i+1, client.Count, client[i]);
            }

            // List<string> all = new List<string>();
            // all.AddRange(client);

            // var clientSb = new StringBuilder();
            // var serverSb = new StringBuilder();

            CreateFile.GenMessageCode(client, args[1]);
            CreateFile.GenBinaryMessagePackerGen(client, args[2]);

            List<string> all = MatchMpc(args[3]);
            List<string> server = new List<string>();
            server.AddRange(client);
            int j = 0;
            foreach(var s in all)
            {
                if (!client.Contains(s))
                {
                    server.Add(s);
                    
                    Console.WriteLine("[Server] {0} of {1} {2}", j + 1, all.Count - client.Count, s);
                    j++;
                }
            }
            CreateFile.GenMessageCode(server, args[4]);
            CreateFile.GenBinaryMessagePackerGen(server, args[5]);
        }
    }
}