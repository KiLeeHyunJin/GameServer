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
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Hellow World {i}");

                ArraySegment<byte> openSegement = SendBufferHelper.Open(sendBuff.Length);
                Array.Copy(sendBuff, 0, openSegement.Array, openSegement.Offset, sendBuff.Length);
                ArraySegment<byte> closeSegement = SendBufferHelper.Close(sendBuff.Length);
                Send(closeSegement);
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
           // Console.WriteLine($"Transferred bytes : {numOfByte}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            const int portNum = 55555;

            string domain = "pkc-5000.shop";
            string hostName = Dns.GetHostName(); //로컬 호스트 이름 가져옴
            
            IPAddress serverCom = IPAddress.Parse("52.79.72.106");
            IPAddress[] addresses = Dns.GetHostAddresses(domain);//해당 호스트의 IP엔트리를

            Thread.Sleep(1000);

            try
            {
                foreach (var address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPEndPoint remoteEndPoint = new IPEndPoint(address, portNum); //최종 주소(첫번쨰 주소의 7777포트번호)

                        Console.WriteLine(remoteEndPoint);

                        Connector connector = new Connector();
                        connector.Connect(remoteEndPoint, () => { return new GameSession(); });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            while (true)
            {
               
            }
        }
    }

}
