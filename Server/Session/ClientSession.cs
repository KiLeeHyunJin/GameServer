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
        public GameRoom Room { get; set; }
        public int SessionId { get; set; }


        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected Client : {endPoint}");
            Program.Room.Push(() => { Program.Room.Enter(this); });
        }


        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfByte)
        {

        }


        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            {
                Program.Room.Push(() => 
                {
                    GameRoom room = Room;
                    room.Leave(this);
                    Room = null;
                });
            }
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }
    }
}
