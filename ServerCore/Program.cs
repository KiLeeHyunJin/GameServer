using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Sever!");
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {

            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfByte)
        {
            Console.WriteLine($"Transferred bytes : {numOfByte}");

        }
    }
    internal class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName(); //로컬 호스트 이름 가져옴
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);//해당 호스트의 IP엔트리를
            IPAddress address = ipHost.AddressList[0];//첫번째 주소를 가져옴
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //최종 주소(첫번쨰 주소의 7777포트번호)

            //문지기 휴대폰 생성


            _listener.Init(endPoint, ()=> { return new GameSession(); });
            Console.WriteLine("Listening...");


            while (true)
            {
                ;
            }

           
            Console.ReadLine();
        }
       
    }
}
