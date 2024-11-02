using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
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

        public List<int> skills = new();

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            int count = 4;
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            
            this.playerId = BitConverter.ToInt64(s.Slice(count,s.Length - count));

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
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
            //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            //success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            //Array.Copy(Encoding.Unicode.GetBytes(name, 0, nameLen, segment.Array, segment.Offset + count);
            //
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
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


    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected Server : {endPoint}");
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Sever!");

            PlayerInfoReq packet = new PlayerInfoReq() {    playerId = 1001, name = "테스트" };


            #region
            //byte[] size = BitConverter.GetBytes(packet.size);
            //byte[] packetId = BitConverter.GetBytes(packet.packetId);
            //byte[] playerId = BitConverter.GetBytes(packet.playerId);

            //Array.Copy(size     , 0, s.Array, s.Offset + count, size.Length);
            //count += 2;

            //Array.Copy(packetId , 0, s.Array, s.Offset + count, packetId.Length);
            //count += 2;

            //Array.Copy(playerId , 0, s.Array, s.Offset + count, playerId.Length);
            //count += 8;
            #endregion

            ArraySegment<byte> s = packet.Write();
            if(s != null)
            {
                Send(s);
            }


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
            ushort id   = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId : {id} size {size}");

            //string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            //Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfByte)
        {
            //Console.WriteLine($"Transferred bytes : {numOfByte}");
        }
    }
}
