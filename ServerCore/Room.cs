using System.Net;
using System.Text;

namespace ServerCore
{

    public class Room : Session
    {
        public bool Finish { get; private set; }
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

}
