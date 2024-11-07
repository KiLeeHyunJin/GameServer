using ServerCore;

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

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new();


    public void Register()
    {
       _onRecv.Add((ushort)PacketID.S_BanPick, MakePacket<S_BanPick>);
        _handler.Add((ushort)PacketID.S_BanPick, PacketHandler.S_BanPickHandler);
       _onRecv.Add((ushort)PacketID.S_PickUp, MakePacket<S_PickUp>);
        _handler.Add((ushort)PacketID.S_PickUp, PacketHandler.S_PickUpHandler);
       _onRecv.Add((ushort)PacketID.S_Attck, MakePacket<S_Attck>);
        _handler.Add((ushort)PacketID.S_Attck, PacketHandler.S_AttckHandler);
       _onRecv.Add((ushort)PacketID.S_Chat, MakePacket<S_Chat>);
        _handler.Add((ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler);
       _onRecv.Add((ushort)PacketID.S_Result, MakePacket<S_Result>);
        _handler.Add((ushort)PacketID.S_Result, PacketHandler.S_ResultHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        int count = 0;
        ushort size = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> action = null;
        if(_onRecv.TryGetValue(id, out action))
        {
            action.Invoke(session, buffer);
        }

    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
        {
            action.Invoke(session, pkt);
        }
    }
}
