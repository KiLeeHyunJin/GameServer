﻿using System.Net;
using System.Net.Sockets;

namespace ServerCore.Base
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;
        Lobby lobby;
        public void Init(Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(
                Define.AddressType,
                Define.SocketType,
                Define.ProtocolType);
            //_listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // 재사용 설정 추가

            _listenSocket.Bind(new IPEndPoint(IPAddress.Any, Define.PortNum));
            _listenSocket.Listen(10);

            Console.WriteLine($"Open Socket {IPAddress.Any}:{Define.PortNum}");
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAccpetCompleted);
            _sessionFactory += sessionFactory;

            lobby = new();

            RegisterAccept(args);
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
                Session session = null;
                session = lobby.EnterLobby(args.AcceptSocket, args.AcceptSocket.RemoteEndPoint);
                //session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            RegisterAccept(args);
        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }

    }
}