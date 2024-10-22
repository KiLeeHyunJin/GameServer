using ServerCore;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{

    internal class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iPHost.AddressList[0];
            IPEndPoint endPoint = new(ipAddr, 7777);
        }

    
    }
}
