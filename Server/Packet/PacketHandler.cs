using Server;
using ServerCore;
using System.Text.RegularExpressions;

internal class PacketHandler
{

    private static bool ClientSessionCheck(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if(clientSession == null)
        {
            Console.WriteLine($"ClientSession is Invalid {(PacketID)packet.Protocol}");
            return false;
        }
        if (clientSession.Room == null)
        {
            Console.WriteLine($"Room is Invalid {clientSession.SessionId} {(PacketID)packet.Protocol}");
            return false;
        }
        return true;
    }
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat p = packet as C_Chat;
        if (ClientSessionCheck(session, packet))
        {
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Chat(clientSession, p));
        }
        //Console.WriteLine("Chat Handler");
    }

    public static void C_EndGameHandler(PacketSession session, IPacket packet)
    {
        C_EndGame p = packet as C_EndGame;
        if (ClientSessionCheck(session, packet))
        {
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Result(clientSession));
        }
    }

    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet))
        {
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Leave(clientSession));
        }
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet))
        {
            //ClientSession clientSession = session as ClientSession;
            //GameRoom room = clientSession.Room;
            C_Move p = packet as C_Move;
            Console.WriteLine($"{p.posX},{p.posZ}");
        }
    }


    public static void C_BanPickHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet))
        {
            C_BanPick p = packet as C_BanPick;
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Ban(clientSession, p));
        }
    }

    public static void C_PickUpHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet))
        {
            C_PickUp p = packet as C_PickUp;
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Pick(clientSession, p));
        }
    }

    public static void C_AttckHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet))
        {
            C_Attck p = packet as C_Attck;
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Attack(clientSession, p));
        }
    }



}
