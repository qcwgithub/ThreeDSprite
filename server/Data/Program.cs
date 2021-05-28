using System;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;

namespace Data
{
    public class Program
    {
        static void Main(string[] args)
        {
            var globalData = new GlobalData();

            // load dll
            var assembly = Assembly.LoadFile(args[0]);
            var scriptEntry = (IScriptEntry) assembly.CreateInstance("Script.ScriptEntry");
            if (!scriptEntry.OnLoad(args, globalData))
            {
                Console.WriteLine("!scriptEntry.OnLoad()");
                return;
            }

            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    ET.ThreadSynchronizationContext.Instance.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}