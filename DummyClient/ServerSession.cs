using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;

    }

    class PlaeyrInfoReq : Packet
    {

    }
    class PlayerInfoOk : Packet
    {

    }

    class ServerSession : PacketSession
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

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId : {id} size {size}");

            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfByte)
        {
            //Console.WriteLine($"Transferred bytes : {numOfByte}");

        }
    }
}
