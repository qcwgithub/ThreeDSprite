using System;
using System.Threading;
using System.Collections.Generic;

public class Program
{
    static void Main(string[] args)
    {        
        //------------------------
        // 异步方法全部会回掉到主线程
        SynchronizationContext.SetSynchronizationContext(ET.ThreadSynchronizationContext.Instance);

        List<Server> servers = ServerCreator.Create(args);
        foreach (Server server in servers)
        {
            server.dispatcher.dispatch(null, MsgType.Start, new object(), null);
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