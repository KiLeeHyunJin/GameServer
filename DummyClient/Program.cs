using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"[Connect To] {endPoint.ToString()}");

            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuffet = Encoding.UTF8.GetBytes($"Hellow World {i}");
                Send(sendBuffet);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {

            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Sever] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfByte)
        {
            Console.WriteLine($"Transferred bytes : {numOfByte}");

        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName(); //로컬 호스트 이름 가져옴
            //Console.WriteLine(hostName);
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);//해당 호스트의 IP엔트리를
            IPAddress address = ipHost.AddressList[0];//첫번째 주소를 가져옴
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //최종 주소(첫번쨰 주소의 7777포트번호)
            
            Thread.Sleep(100);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new GameSession(); });

            while (true)
            {
                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
           
            Console.ReadLine();
        }
    }

}
