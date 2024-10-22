using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{


    internal class Program
    {
        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName(); //로컬 호스트 이름 가져옴
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);//해당 호스트의 IP엔트리를
            IPAddress address = ipHost.AddressList[0];//첫번째 주소를 가져옴
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //최종 주소(첫번쨰 주소의 7777포트번호)

            try
            {
                //휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //입장 요청
                socket.Connect(endPoint);

                Console.WriteLine($"[Connect To] {socket.RemoteEndPoint.ToString()}");
                byte[] sendBuffet = Encoding.UTF8.GetBytes("Hellow World");
                int sendBytes = socket.Send(sendBuffet);

                byte[] recvBuffer = new byte[1024];
                int recBytes = socket.Receive(recvBuffer);
                string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recBytes);

                Console.WriteLine($"From Server {recvData}");
                //socket.Shutdown(SocketShutdown.Both);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }

}
