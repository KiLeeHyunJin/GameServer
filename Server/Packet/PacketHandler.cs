using Server;
using ServerCore;

internal class PacketHandler
{

    private static bool ClientSessionCheck(PacketSession session, ushort packetId)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null)
        {
            Console.WriteLine($"ClientSession is Invalid {(PacketID)packetId}");
            return false;
        }
        if (clientSession.Room == null)
        {
            Console.WriteLine($"Room is Invalid {clientSession.SessionId} {(PacketID)packetId}");
            return false;
        }
        return true;
    }

    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat p = packet as C_Chat;
        if (ClientSessionCheck(session, packet.Protocol))
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
        if (ClientSessionCheck(session, packet.Protocol))
        {
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Result(clientSession));
        }
    }

    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet.Protocol))
        {
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Leave(clientSession));
        }
    }

    public static void C_LastBanPickHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet.Protocol))
        {
            C_LastBanPick p = packet as C_LastBanPick;
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.LastBan(clientSession, p));
        }
    }


    public static void C_BanPickHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet.Protocol))
        {
            C_BanPick p = packet as C_BanPick;
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Ban(clientSession, p));
        }
    }

    public static void C_PickUpHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet.Protocol))
        {
            C_PickUp p = packet as C_PickUp;
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Pick(clientSession, p));
        }
    }

    public static void C_AttckHandler(PacketSession session, IPacket packet)
    {
        if (ClientSessionCheck(session, packet.Protocol))
        {
            C_Attck p = packet as C_Attck;
            ClientSession clientSession = session as ClientSession;
            GameRoom room = clientSession.Room;
            room.Push(() => room.Attack(clientSession, p));
        }
    }



}
