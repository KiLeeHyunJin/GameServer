using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Define
    {
        readonly public static AddressFamily    AddressType         = AddressFamily.InterNetwork;
        readonly public static SocketType       SocketType          = SocketType.Stream;
        readonly public static ProtocolType     ProtocolType        = ProtocolType.Tcp;

        readonly public static int              PortNum             = 55555;
        readonly public static int              SendBufferChunkSize = 4096 * 100;
        public enum PacketID
        {
            PlayerInfoReq = 1,
            PlayerInfoOk = 2,
        }


    }
}
