using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
            {
            Console.WriteLine($"");
            for (int i = 0; i < 5; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes("Welcome");
                int sendByte = socket.Send(bytes);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"On Disconneted");
        }

        public override void OnRecv(ArraySegment<byte> buff)
        {
            Console.WriteLine($"{Encoding.UTF8.GetString(buff.Array, buff.Offset, buff.Count)}");
        }

        public override void OnSend(int byteSize)
        {
            Console.WriteLine($" Transferred : {byteSize}");
        }
    }


    internal class Program
    {

        static void SendWait()
        {
            while (true)
            {

            }
        }

        static void ReciveWait()
        {
            while (true)
            {       

            }
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iPHost.AddressList[0];
            IPEndPoint endPoint = new(ipAddr, 7777);

            Socket socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endPoint);

            Console.WriteLine($"Connected To {socket.RemoteEndPoint}");


            byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World");
            int sendBytes = socket.Send(sendBuff);

            byte[] recvBuff = new byte[1024];
            int recvBytes = socket.Receive(recvBuff);
            string recvString = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
            Console.WriteLine($"[From Server] {recvString}");
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();


        }
    }
}
