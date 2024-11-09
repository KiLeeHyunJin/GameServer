using Server;
using ServerCore;
using System.Text.RegularExpressions;

internal class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat p = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Room == null)
        {   return; }

        GameRoom room = clientSession.Room;
        //Console.WriteLine("Chat Handler");
        room.Push(() => room.Chat(clientSession, p));
    }

    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if(clientSession == null)
        {
            return;
        }
        GameRoom room = clientSession.Room;
        //Console.WriteLine("C_LeaveGameHandler");

        room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move p = packet as C_Move;
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null)
        {
            return;
        }
        GameRoom room = clientSession.Room;
        if(room == null)
        {
            Console.WriteLine($"Room is Invalid {clientSession.SessionId}");
        }
        else if(p == null)
        {
            Console.WriteLine($"Packet is Invalid");
        }
        else
        {
            //Console.WriteLine($"{p.posX},{p.posZ}")
            room.Push(() => room.Move(clientSession, p));
        }
    }


    public static void C_BanPickHandler(PacketSession session, IPacket packet)
    {
        C_BanPick p = packet as C_BanPick;
    }

    public static void C_PickUpHandler(PacketSession session, IPacket packet)
    {
        C_PickUp p = packet as C_PickUp;
    }

    public static void C_AttckHandler(PacketSession session, IPacket packet)
    {
        C_Attck p = packet as C_Attck;
    }



}
