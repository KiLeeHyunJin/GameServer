using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class PacketHandler
{
    public static void S_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;
        Console.WriteLine($"PlayerInfoReq Player ID : {p.playerId} name : {p.name}");
        foreach (var skill in p.skills)
        {
            Console.WriteLine($"Skill(Id:{skill.id} / level:{skill.level} / duration:{skill.duration})");
        }
    }

    public static void S_TestHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;
        Console.WriteLine($"PlayerInfoReq Player ID : {p.playerId} name : {p.name}");
        foreach (var skill in p.skills)
        {
            Console.WriteLine($"Skill(Id:{skill.id} / level:{skill.level} / duration:{skill.duration})");
        }
    }
}
