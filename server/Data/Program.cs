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
            var scriptEntry = (IScriptEntry)assembly.CreateInstance("Script.ScriptEntry");
            if (!scriptEntry.OnLoad(args, globalData))
            {
                Console.WriteLine("!scriptEntry.OnLoad()");
                return;
            }

            // DateTime time = DateTime.Now;
            // bool loadAgain = true;
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    ET.ThreadSynchronizationContext.Instance.Update();

                    // if (loadAgain && DateTime.Now.Subtract(time).TotalSeconds >= 5)
                    // {
                    //     loadAgain = false;
                    //     var assembly2 = Assembly.LoadFile(args[0]);
                    //     var scriptEntry2 = (IScriptEntry)assembly2.CreateInstance("Script.ScriptEntry");
                    //     if (!scriptEntry2.OnLoad(args, globalData))
                    //     {
                    //         Console.WriteLine("!scriptEntry.OnLoad()");
                    //         return;
                    //     }
                    // }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}