using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected Client : {endPoint}");
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


            switch ((Define.PacketID)id)
            {
                case Define.PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq Player ID : {p.playerId} name : {p.name}");
                        foreach (var skill in p.skills)
                        {
                            Console.WriteLine($"Skill(Id:{skill.id} / level:{skill.level} / duration:{skill.duration})");
                        }
                    
                    }
                    break;
                case Define.PacketID.PlayerInfoOk:
                    {

                    }
                    break;
            }

            Console.WriteLine($"RecvPacketId : {id} size {size}");
        }

        public override void OnSend(int numOfByte)
        {

        }
    }
}
