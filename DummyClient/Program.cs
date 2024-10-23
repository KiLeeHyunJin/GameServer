using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{


    internal class Program
    {
        static void Main(string[] args)
        {
            //string hostName = Dns.GetHostName(); //로컬 호스트 이름 가져옴
            //Console.WriteLine(hostName);
            IPHostEntry ipHost = Dns.GetHostEntry("DESKTOP-CQOT7QA");//해당 호스트의 IP엔트리를
            IPAddress address = ipHost.AddressList[0];//첫번째 주소를 가져옴
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //최종 주소(첫번쨰 주소의 7777포트번호)

            while(true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


                try
                {
                    //휴대폰 설정

                    //입장 요청
                    socket.Connect(endPoint);

                    Console.WriteLine($"[Connect To] {socket.RemoteEndPoint.ToString()}");

                    for (int i = 0; i < 5; i++)
                    {

                        byte[] sendBuffet = Encoding.UTF8.GetBytes($"Hellow World {i}");
                        int sendBytes = socket.Send(sendBuffet);
                    }

                    byte[] recvBuffer = new byte[1024];
                    int recBytes = socket.Receive(recvBuffer);
                    string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recBytes);

                    Console.WriteLine($"From Server {recvData}");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(100);
            }
           
            Console.ReadLine();
        }
    }

}
