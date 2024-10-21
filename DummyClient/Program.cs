using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
   

    internal class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iPHost.AddressList[0];
            IPEndPoint endPoint = new(ipAddr, 7777);

            Connector connector = new();
            connector.Connect(endPoint, () => { return new GameSession(); });

            while(true)
            {
                Socket socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connected To {socket.RemoteEndPoint}");


                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch(Exception e)
                {

                }
            }
            
        }
    }
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"On Connected : {endPoint}");
            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World");
                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"On Disconneted");
        }

        public override int OnRecv(ArraySegment<byte> buff)
        {
            Console.WriteLine($"[From Server] : {Encoding.UTF8.GetString(buff.Array, buff.Offset, buff.Count)}");
            return buff.Count;
        }

        public override void OnSend(int byteSize)
        {
            Console.WriteLine($" Transferred : {byteSize}");
        }
    }

}
