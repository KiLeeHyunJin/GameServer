using System;
using System.Net;
using System.Net.Sockets;


namespace ServerCore
{
    public class Connector
    {
        Func<Session> _sessionFactory;
        //Lobby lobby = new Lobby();

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, Define.Connect connect, int count = 1)
        {
            _sessionFactory = sessionFactory;

            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(
                Define.AddressType,
                Define.SocketType,
                Define.ProtocolType);
                //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // ���� ���� �߰�

                socket.ReceiveTimeout = 30000;
                if(connect == Define.Connect.Domain)
                {
                    //socket.Bind(new IPEndPoint(IPAddress.Any, Define.PortNum));
                }
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();

                args.UserToken = socket;
                args.RemoteEndPoint = endPoint;
                args.Completed += OnConnectedCompleted;

                RegistConnect(args);
            }
          
        }

        void RegistConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
            {
                Console.WriteLine($"socket invalid exception");
                return;
            }
            bool pending = socket.ConnectAsync(args);
            if (pending == false)
            {
                OnConnectedCompleted(null, args);
            }
        }

        void OnConnectedCompleted(object? sender, SocketAsyncEventArgs args)
        {
            Console.WriteLine("OnConnectedCompleted");
            if (args.SocketError == SocketError.Success)
            {
                //lobby.EnterLobby(args.ConnectSocket, args.RemoteEndPoint);
                Session session = _sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine("OnConnectedCompleted Failed");
            }
        }
    }

}