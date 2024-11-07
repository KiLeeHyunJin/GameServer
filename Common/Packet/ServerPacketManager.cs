using Server;
using ServerCore;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance;
    public static PacketManager Instance
    {
        get { return _instance ??= new PacketManager(); }
    }
    #endregion Singleton

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new();


    public void Register()
    {
       _onRecv.Add((ushort)PacketID.C_PlayerInfoReq, MakePacket<C_PlayerInfoReq>);
        _handler.Add((ushort)PacketID.C_PlayerInfoReq, PacketHandler.C_PlayerInfoReqHandler);
       _onRecv.Add((ushort)PacketID.C_Test, MakePacket<C_Test>);
        _handler.Add((ushort)PacketID.C_Test, PacketHandler.C_TestHandler);

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
        if (_handler.TryGetValue(pkt.Prptocol, out action))
        {
            action.Invoke(session, pkt);
        }
    }
}
