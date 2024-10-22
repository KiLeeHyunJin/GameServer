using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName(); //로컬 호스트 이름 가져옴
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);//해당 호스트의 IP엔트리를
            IPAddress address = ipHost.AddressList[0];//첫번째 주소를 가져옴
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //최종 주소(첫번쨰 주소의 7777포트번호)

            //문지기 휴대폰 생성
            Socket socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(10);

            try
            {
                while (true)
                {
                    Console.WriteLine("Listening");
                    //손님 입장
                    Socket clientSocket = socket.Accept(); //손님 입장 안할 시 대기

                    byte[] recvBuffer = new byte[1024];
                    int recvBytes = clientSocket.Receive(recvBuffer);
                    string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);

                    Console.WriteLine($"[From Client] {recvData}");

                    byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
                    clientSocket.Send(sendBuffer);

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
       
    }
}
