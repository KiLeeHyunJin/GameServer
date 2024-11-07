using Server;
using ServerCore;

internal class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat p = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Room == null)
        {   return; }

        GameRoom room = clientSession.Room;
        room.Push(
            () => { room.Broadcast(clientSession, p.chat); }
            );
    }


    public static void C_MatchHandler(PacketSession session, IPacket packet)
    {
        C_Match p = packet as C_Match;
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
