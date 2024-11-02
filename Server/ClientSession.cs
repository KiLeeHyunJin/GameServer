using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;
        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            int count = 0;
            //ushort size = (ushort)BitConverter.ToInt16(s.Array, s.Offset);
            count += 2;
            //ushort id = (ushort)BitConverter.ToInt16(s.Array, s.Offset + count);
            count += 2;

            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);
            short count = 0;

            bool success = true;

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);

            count += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            if (success == false)
            {
                return null;
            }

            ArraySegment<byte> closeSegement = SendBufferHelper.Close(count);
            return s;
        }
    }


    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }


    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Sever!");

            //ArraySegment<byte> openSegement = SendBufferHelper.Open(sendBuff.Length);
            //Array.Copy(sendBuff, 0, openSegement.Array, openSegement.Offset, sendBuff.Length);
            //ArraySegment<byte> closeSegement = SendBufferHelper.Close(sendBuff.Length);
            //Send(closeSegement);


            //Thread.Sleep(1000);
            //Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            int count = 0;
            ushort size = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset + count);
            count += 2;


            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq Player ID : {p.playerId}");
                    }
                    break;
                case PacketID.PlayerInfoOk:
                    {

                    }
                    break;
            }

            //ushort size     = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            //ushort id       = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

            //string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            
            Console.WriteLine($"RecvPacketId : {id} size {size}");
            //Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfByte)
        {
            //Console.WriteLine($"Transferred bytes : {numOfByte}");

        }
    }
}
