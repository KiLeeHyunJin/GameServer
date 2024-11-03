using ServerCore.Base;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{

    internal class Program
    {

        static void Main(string[] args)
        {
            string domain = "pkc-5000.shop";
            string local = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(local);

            Thread.Sleep(1000);

            try
            {
                foreach (var address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPEndPoint remoteEndPoint = new IPEndPoint(address, ServerCore.Define.PortNum);
                        Console.WriteLine($"[RemoteAddress] : {remoteEndPoint} ");

                        Connector connector = new Connector();
                        connector.Connect(remoteEndPoint, () => { return new ServerSession(); });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); 
            }


            while (true) {   }
        }
    }

}
