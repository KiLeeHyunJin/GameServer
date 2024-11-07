using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class BattleRoom : PacketSession
    {
        public bool Finish { get; private set; }
        Socket[] sockets;
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

        public void OnPlay()
        {

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


            //switch ((Define.PacketID)id)
            //{
            //    case Define.PacketID.PlayerInfoReq:
            //        {
            //            PlayerInfoReq p = new PlayerInfoReq();
            //            p.Read(buffer);
            //            Console.WriteLine($"PlayerInfoReq Player ID : {p.playerId} name : {p.name}");
            //            foreach (var skill in p.skills)
            //            {
            //                Console.WriteLine($"Skill(Id:{skill.id} / level:{skill.level} / duration:{skill.duration})");
            //            }

            //        }
            //        break;
            //    case Define.PacketID.PlayerInfoOk:
            //        {

            //        }
            //        break;
            //}

            ////ushort size     = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ////ushort id       = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

            ////string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);

            //Console.WriteLine($"RecvPacketId : {id} size {size}");
            ////Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfByte)
        {
            //Console.WriteLine($"Transferred bytes : {numOfByte}");

        }
    }

}
