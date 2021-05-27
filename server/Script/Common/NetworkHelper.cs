using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Model
{
    public static class NetworkHelper
    {
        private static readonly HashSet<ushort> needDebugLogMessageSet = new HashSet<ushort> { 1 };

        public static IPEndPoint ToIPEndPoint(string host, int port)
        {
            return new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static IPEndPoint ToIPEndPoint(string address)
        {
            int index = address.LastIndexOf(':');
            string host = address.Substring(0, index);
            string p = address.Substring(index + 1);
            int port = int.Parse(p);
            return ToIPEndPoint(host, port);
        }

        public static string[] GetAddressIPs()
        {
            //获取本地的IP地址
            List<string> addressIPs = new List<string>();
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    addressIPs.Add(address.ToString());
                }
            }
            return addressIPs.ToArray();
        }

        public static bool IsNeedDebugLogMessage(ushort opcode)
        {
            return false;

            if (opcode > 1000)
            {
                return true;
            }

            if (needDebugLogMessageSet.Contains(opcode))
            {
                return true;
            }

            return false;
        }

        public static bool IsClientHotfixMessage(ushort opcode)
        {
            return opcode > 10000;
        }

        public static void Close(this HttpListenerContext context)
        {
            try
            {
                context?.Response.Close();
            }
            catch
            {
                context?.Response.Abort();
            }
        }

        public static string GetIp(this HttpListenerContext context)
        {
            string ip = context.Request.RemoteEndPoint.Address.ToString();
            try
            {
                var forwarded = context.Request.Headers.Get("X-Forwarded-For");
                if (forwarded != null)
                {
                    var arr = forwarded.Split(',');
                    if (arr != null && arr.Length > 0)
                        ip = arr[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return ip;
        }
    }
}
