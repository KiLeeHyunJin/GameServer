using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    internal class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                //Console.WriteLine($"[From Client] {recvData}");

                Session session = new Session();
                session.Start(clientSocket);

                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
                session.Send(sendBuffer);

                Thread.Sleep(1000);

                session.Disconnect();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName(); //로컬 호스트 이름 가져옴
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);//해당 호스트의 IP엔트리를
            IPAddress address = ipHost.AddressList[0];//첫번째 주소를 가져옴
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //최종 주소(첫번쨰 주소의 7777포트번호)

            //문지기 휴대폰 생성


            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");


            while (true)
            {
                ;
            }

           
            Console.ReadLine();
        }
       
    }
}
