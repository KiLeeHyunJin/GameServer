using ServerCore;
using System.Net;
using System.Text;

namespace Server
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Sever!");

            ArraySegment<byte> openSegement = SendBufferHelper.Open(sendBuff.Length);
            Array.Copy(sendBuff, 0, openSegement.Array, openSegement.Offset, sendBuff.Length);
            ArraySegment<byte> closeSegement = SendBufferHelper.Close(sendBuff.Length);
            Send(closeSegement);


            //Thread.Sleep(1000);
            //Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfByte)
        {
            //Console.WriteLine($"Transferred bytes : {numOfByte}");

        }
    }


    internal class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            try
            {
                _listener.Init(() => { return new GameSession(); });
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }

}
