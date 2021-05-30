using System;
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

    public class Program
    {
        static WeakReference LoadAssembly(string[] args, DataEntry globalData)
        {
            var context = new MyAssemblyLoadContext();
            var weakRef = new WeakReference(context);
            var assembly = context.LoadFromAssemblyPath(args[0]);
            var scriptEntry = (IScriptEntry)assembly.CreateInstance("Script.ScriptEntry");
            scriptEntry.OnLoad(args, globalData);
            assembly = null;
            context.Unload();
            return weakRef;
        }

        static void Main(string[] args)
        {
            var globalData = new DataEntry();
            var contextList = new List<WeakReference>();

            contextList.Add(LoadAssembly(args, globalData));
            //PrintInfo();

            DateTime time = DateTime.Now;
            bool loadAgain = true;
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    ET.ThreadSynchronizationContext.Instance.Update();

                //     if (loadAgain)
                //     {
                //         if (DateTime.Now.Subtract(time).TotalSeconds >= 5)
                //         {
                //             loadAgain = false;
                //             contextList.Add(LoadAssembly(args, globalData));
                //         }
                //     }
                //     else
                //     {
                //         DateTime now = DateTime.Now;
                //         if (now.Subtract(time).TotalSeconds >= 2)
                //         {
                //             time = now;
                //             //PrintInfo();
                //             for (int i = 0; i < contextList.Count; i++)
                //             {
                //                 if (!contextList[i].IsAlive)
                //                 {
                //                     contextList.RemoveAt(i);
                //                     i--;
                //                 }
                //             }
                //             Console.WriteLine("Alive count: " + contextList.Count);
                //             GC.Collect();
                //             // GC.WaitForPendingFinalizers();
                //         }
                //     }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}