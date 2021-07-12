using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;

namespace Data
{
    // https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
    class MyAssemblyLoadContext : AssemblyLoadContext
    {
        public MyAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName name)
        {
            // As you can see, the Load method returns null. 
            // That means that all the dependency assemblies are loaded into the default context, 
            // and the new context contains only the assemblies explicitly loaded into it.
            return null;
        }
    }

    public class ScriptEntryAndContextWeakRef
    {
        public IScriptEntry scriptEntry;
        public int scriptDllVersion;
        public WeakReference contextWeakRef;
    }

    public class Program
    {
        public static int scriptDllVersion = 1;
        public static bool LoadScriptDll()
        {
            try
            {
                var bytes = File.ReadAllBytes(scriptDllPath);
                var symbols = File.ReadAllBytes(scriptDllPath.Substring(0, scriptDllPath.Length - 4) + ".pdb");

                ScriptEntryAndContextWeakRef last = null;
                if (assemblyLoadContextRefs.Count > 0)
                {
                    last = assemblyLoadContextRefs[assemblyLoadContextRefs.Count - 1];
                }

                var context = new MyAssemblyLoadContext();
                var weakRef = new WeakReference(context);

                Assembly assembly;
                using (var stream = new MemoryStream(bytes))
                {
                    using (var symbolStream = new MemoryStream(symbols))
                    {
                        assembly = context.LoadFromStream(stream, symbolStream);
                    }
                }

                var scriptEntry = (IScriptEntry)assembly.CreateInstance("Script.ScriptEntry");
                scriptEntry.OnLoad(args, dataEntry, scriptDllVersion++);
                assembly = null;
                // context.Unload();

                assemblyLoadContextRefs.Add(
                    new ScriptEntryAndContextWeakRef
                    {
                        scriptEntry = scriptEntry,
                        scriptDllVersion = scriptEntry.GetVersion(),
                        contextWeakRef = weakRef,
                    });

                if (last != null)
                {
                    last.scriptEntry.OnUnload();
                    last.scriptEntry = null; // 最好是置为 null...

                    // 这个可能会挂，不知道原因，Debug 模式
                    // 不过在 Debug 下，不调用 Unload ，测试加载 9000 次也没涨内存
                    // if (last.Item2.IsAlive)
                    // {
                    //     ((MyAssemblyLoadContext)last.Item2.Target).Unload();
                    // }
                }

                checkTime = DateTime.Now.AddSeconds(-1);

                return true;
            }
            catch (Exception ex)
            {
                LogToLoggerOrConsole("LoadScriptDll exception ", ex, "error");
                return false;
            }
        }

        public static void LogToLoggerOrConsole(string message, Exception ex, string type)
        {
            bool logged = false;
            if (dataEntry != null && dataEntry.serverDatas != null && dataEntry.serverDatas.Count > 0)
            {
                foreach (var kv in dataEntry.serverDatas)
                {
                    if (kv.Value.logger != null)
                    {
                        logged = true;
                        if (type == "error")
                        {
                            kv.Value.logger.Error(message, ex);
                        }
                        else
                        {
                            kv.Value.logger.Info(message, ex);
                        }
                    }
                    break;
                }
            }

            if (!logged)
            {
                Console.WriteLine(message + ex);
            }
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogToLoggerOrConsole("unhandled exception ", (Exception)e.ExceptionObject, "error");
        }


        static Dictionary<string, string> ParseArguments(string[] args)
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
                    // Console.WriteLine(key + ": " + value);
                    argMap.Add(key, value);
                }
            }
            return argMap;
        }

        public static Dictionary<string, string> args;
        public static List<ScriptEntryAndContextWeakRef> assemblyLoadContextRefs;
        public static string scriptDllPath;
        public static DataEntry dataEntry;
        public static DateTime checkTime;
        public static DateTime updateTime;

        // scriptDll=./xx/Script.dll ids=all purpose=Test
        public static void Main(string[] args)
        {
            Console.WriteLine();
#if DEBUG
            Console.WriteLine("**** Configuration: Debug");
#else
            Console.WriteLine("**** Configuration: Release");
#endif

            Console.WriteLine("**** Working Directory: " + Directory.GetCurrentDirectory());
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("**** Arguments[{0}] {1}", i, args[i]);
            }

            Program.args = ParseArguments(args);
            scriptDllPath = Program.args["scriptDll"];
            dataEntry = new DataEntry();
            assemblyLoadContextRefs = new List<ScriptEntryAndContextWeakRef>();

            // unhandled exception
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            // 异步方法全部会回掉到主线程
            SynchronizationContext.SetSynchronizationContext(ET.ThreadSynchronizationContext.Instance);

            LoadScriptDll();

            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    ET.ThreadSynchronizationContext.Instance.Update();

                    var refs = assemblyLoadContextRefs;
                    if (refs.Count > 0)
                    {
                        var last = refs[refs.Count - 1];
                        if (last.scriptEntry.needUpdate)
                        {
                            DateTime now = DateTime.Now;
                            TimeSpan span = now - updateTime;
                            double ms = span.TotalSeconds;
                            if (ms >= 0.03333f)
                            {
                                updateTime = now;
                                last.scriptEntry.Update(0.03333f);
                            }
                        }
                    }
                    if (refs.Count > 1)
                    {
                        DateTime now = DateTime.Now;
                        var sb = new StringBuilder();
                        if (now.Subtract(checkTime).TotalSeconds > 1)
                        {
                            checkTime = now;
                            for (int i = 0; i < refs.Count; i++)
                            {
                                if (!refs[i].contextWeakRef.IsAlive)
                                {
                                    refs.RemoveAt(i);
                                    i--;
                                }
                                else
                                {
                                    sb.AppendFormat("V{0} ", refs[i].scriptDllVersion);
                                }
                            }
                            LogToLoggerOrConsole("AssemblyLoadContextRefs.Count = " + (refs.Count) + ", " + sb.ToString(), null, "info");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogToLoggerOrConsole("main loop exception", ex, "error");
                }
            }
        }
    }
}