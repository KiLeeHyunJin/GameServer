using ServerCore;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
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

        public override int OnRecv(ArraySegment<byte> buff)
        {
            Console.WriteLine($"{Encoding.UTF8.GetString(buff.Array, buff.Offset, buff.Count)}");
            return buff.Count;
        }

        public override void OnSend(int byteSize)
        {
            Console.WriteLine($" Transferred : {byteSize}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = iPHost.AddressList[0];
            IPEndPoint endPoint = new(ipAddr, 7777);
            Listener listener = new(endPoint, () => { return new GameSession(); });
            while (true)
            { }
        }

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                byte[] recvBuff = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Client] {recvData}");

                byte[] sendBuff = Encoding.UTF8.GetBytes("WelCome");
                clientSocket.Send(sendBuff);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
