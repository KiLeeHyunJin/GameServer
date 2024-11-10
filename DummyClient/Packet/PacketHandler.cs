using DummyClient;
using ServerCore;
using System.Diagnostics;
//using UnityEngine;

internal class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat p = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;
        Console.WriteLine($"{p.playerId} : {p.chat}");
        //Debug.Log($"{p.playerId} : {p.chat}");
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame p = packet as S_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;
        Console.WriteLine($"Exit Player Id : {p.playerId}");
    }

    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame p = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
        Console.WriteLine($"Enter Player Id : {p.playerId}");
    }
    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove p = packet as S_BroadcastMove;
        ServerSession serverSession = session as ServerSession;
    }
    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList p = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;
    }

    public static void S_BanPickHandler(PacketSession session, IPacket packet)
    {
        S_BanPick p = packet as S_BanPick;
        ServerSession serverSession = session as ServerSession;
        Console.WriteLine($"Ban Player ID : {p.playerId}, Ban Idx : {p.banId}");

    }

    public static void S_PickUpHandler(PacketSession session, IPacket packet)
    {
        S_PickUp p = packet as S_PickUp;
        ServerSession serverSession = session as ServerSession;
        Console.WriteLine($"Pick Player ID : {p.playerId}, Pick Idx : {p.pickIdx}");


    }

    public static void S_AttckHandler(PacketSession session, IPacket packet)
    {
        S_Attck p = packet as S_Attck;
        ServerSession serverSession = session as ServerSession;
        Console.WriteLine($"Attack Player ID : {p.playerId}, Attack Idx : {p.atckId}, Skill Type : {p.skillId}");
    }

    public static void S_ResultHandler(PacketSession session, IPacket packet)
    {
        S_Result p = packet as S_Result;
        ServerSession serverSession = session as ServerSession;
        Console.WriteLine($"EndGame");
    }

}
