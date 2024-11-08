using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;
        Lobby lobby;
        public void Init(Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _sessionFactory -= sessionFactory;
            _sessionFactory += sessionFactory;

            _listenSocket = new Socket(
                Define.AddressType,
                Define.SocketType,
                Define.ProtocolType);
            _listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // 재사용 설정 추가

            _listenSocket.Bind(new IPEndPoint(IPAddress.Any, Define.PortNum));
            _listenSocket.Listen(backlog);

            lobby = new();

            for (int i = 0; i < register; i++)
            {
                Console.WriteLine($"Open Socket {IPAddress.Any}:{Define.PortNum}");
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAccpetCompleted);
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
            {
                OnAccpetCompleted(null, args);
            }
        }

        void OnAccpetCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                //session = lobby.EnterLobby(args.AcceptSocket, args.AcceptSocket.RemoteEndPoint);
                Socket socket = args.AcceptSocket;
                if(socket != null)
                {
                    Session session = _sessionFactory.Invoke();
                    try
                    {
                        Console.WriteLine($"Create Session");
                        session.Start(socket);
                        session.OnConnected(socket.RemoteEndPoint);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Accept Socket Invaild : {e.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Accept Invaild : {args.SocketError.ToString()}");
            }
            RegisterAccept(args);
        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }

    }
}
