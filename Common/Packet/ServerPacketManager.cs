using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance = new();
    public static PacketManager Instance
    {
        get { return _instance; }
    }
    #endregion Singleton
    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new();


    public void Register()
    {
       _makeFunc.Add((ushort)PacketID.C_BanPick, MakePacket<C_BanPick>);
        _handler.Add((ushort)PacketID.C_BanPick, PacketHandler.C_BanPickHandler);
       _makeFunc.Add((ushort)PacketID.C_PickUp, MakePacket<C_PickUp>);
        _handler.Add((ushort)PacketID.C_PickUp, PacketHandler.C_PickUpHandler);
       _makeFunc.Add((ushort)PacketID.C_Attck, MakePacket<C_Attck>);
        _handler.Add((ushort)PacketID.C_Attck, PacketHandler.C_AttckHandler);
       _makeFunc.Add((ushort)PacketID.C_Chat, MakePacket<C_Chat>);
        _handler.Add((ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler);
       _makeFunc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
        _handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);
       _makeFunc.Add((ushort)PacketID.C_EndGame, MakePacket<C_EndGame>);
        _handler.Add((ushort)PacketID.C_EndGame, PacketHandler.C_EndGameHandler);
       _makeFunc.Add((ushort)PacketID.C_Move, MakePacket<C_Move>);
        _handler.Add((ushort)PacketID.C_Move, PacketHandler.C_MoveHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        int count = 0;
        ushort size = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> fnc = null;
        if(_makeFunc.TryGetValue(id, out fnc))
        {
            IPacket p = fnc.Invoke(session, buffer);
            if (onRecvCallback != null)
            {
                onRecvCallback.Invoke(session, p);
            }
            else
            {
                HandlePacket(session, p);
            }
        }

    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        return pkt;
    }

    public void HandlePacket(PacketSession session, IPacket pkt)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
        {
            action.Invoke(session, pkt);
        }
    }
}
