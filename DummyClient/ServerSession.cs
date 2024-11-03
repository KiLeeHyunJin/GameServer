using ServerCore;
using System.Net;

namespace DummyClient
{


    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected Server : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "테스트" };
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 101, duration = 3.0f, level = 1 });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 102, duration = 6.0f, level = 3 });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 103, duration = 9.0f, level = 5 });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 104, duration = 12.0f, level = 7 });

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
            if (s != null)
            {
                Send(s);
            }

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

        }

        public override void OnSend(int numOfByte)
        {

        }
    }
}
