namespace PackterGenerator
{
    public class PacketFormat
    {
        /// <summary>
        /// {0} 패킷 등록
        /// </summary>
        public static string managerFormat =
@"using Server;
using ServerCore;

public class PacketManager
{{
    #region Singleton
    static PacketManager _instance;
    public static PacketManager Instance
    {{
        get {{ return _instance ??= new PacketManager(); }}
    }}
    #endregion Singleton

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new();


    public void Register()
    {{
{0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        int count = 0;
        ushort size = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> action = null;
        if(_onRecv.TryGetValue(id, out action))
        {{
            action.Invoke(session, buffer);
        }}

    }}

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Prptocol, out action))
        {{
            action.Invoke(session, pkt);
        }}
    }}
}}
";
        /// <summary>
        /// {0} 패킷 이름
        /// </summary>
        public static string managerRegisterFormat =
@"       _onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);";


        /// <summary>
        /// {0}패킷 이름
        /// {1}완성된 파일
        /// </summary>
        public static string fileFormat =
@"
using ServerCore;
using ServerCore.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public enum PacketID
{{
    {0}
}}

interface IPacket
{{
	ushort Prptocol {{ get; }}
	void Read(ArraySegment<byte> segement);
	ArraySegment<byte> Write();
}}

{1}
";
        /// <summary>
        /// {0}패킷 이름
        /// {1}패킷 번호
        /// </summary>
        public static string packetEnumFormat =
@"{0} = {1},";

        /// <summary>
        /// {0}패킷 이름
        /// {1}멤버 변수
        /// {2}멤버 변수 Read
        /// {3}멤버 변수 Write
        /// </summary>
        public static string packetFormat =
@"public class {0} : IPacket
{{
    {1}

    public ushort Prptocol {{ get {{return (ushort)PacketID.{0}; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);

        {3}

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }}
}}
";

        #region Member
        /// <summary>
        /// {0}멤버 변수 형식
        /// {1}멤버 변수 이름
        /// </summary>
        public static string memberFormat =
@"public {0} {1};";

        /// <summary>
        /// {0}리스트 이름 {대문자}
        /// {1}리스트 이름 {소믄지}
        /// {2}멤버 변수
        /// {3}멤버 변수 Read
        /// {4}멤버 변수 Write
        /// </summary>
        public static string memberListFormat =
@"
#region {0}
public struct {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}
}}
#endregion {0}
public List<{0}> {1}s = new List<{0}>();
";

        #endregion Member

        #region Read
        /// <summary>
        /// {0}멤버 변수 이름
        /// {1}To - 변수 형식
        /// {2}변수 형식
        /// </summary>
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});
";
        /// <summary>
        /// {0}변수 이름
        /// {1}변수 형식
        /// </summary>
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});
";
        /// <summary>
        /// {0}변수 이름
        /// </summary>
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);

this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;
";
        /// <summary>
        /// {0} 리스트 이름(대문자)
        /// {1} 리스트 이름(소문자)
        /// </summary>
        public static string readListFormat =
@"
this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}
";
        #endregion Read

        #region Write
        /// <summary>
        /// {0}변수 이름
        /// {1}변수 형식
        /// </summary>
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});
";
        /// <summary>
        /// {0}변수 이름
        /// {0}변수 형식
        /// </summary>
        public static string writeByteFormat =
@" segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});
";

        /// <summary>
        /// {0}변수 이름
        /// </summary>
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;
";

        /// <summary>
        /// {0}리스트 이름 {대문자}
        /// {1}리스트 이름 {소믄지}
        /// </summary>
        public static string writeListFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort){1}s.Count);
count += sizeof(ushort);

foreach ({0} {1} in {1}s)
{{
    success &= {1}.Write(s, ref count);
}}
";
        #endregion Write

    }
}
