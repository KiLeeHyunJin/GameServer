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
        public string name;

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            int count = 4;
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            this.playerId   = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen  = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);

            //string
            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(name), 0, segment.Array, count, nameLen);
            count += nameLen;

            success &= BitConverter.TryWriteBytes(s, count);

            return success ? SendBufferHelper.Close(count) : null;
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
            Console.WriteLine($"OnConnected Client : {endPoint}");
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
                        Console.WriteLine($"PlayerInfoReq Player ID : {p.playerId} name : {p.name}");
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
