using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        const int listenWaitMaxValue = 10;
        Socket socket;
        Func<Session> sessonFactory;
        object lck = new();
        public Listener(IPEndPoint endPoint, Func<Session> sessonFactory)
        {
            Init(endPoint, sessonFactory);
        }

        public void Init(IPEndPoint endPoint, Func<Session> sessonFactory)
        {
            this.socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sessonFactory = sessonFactory;
            socket.Bind(endPoint);
            socket.Listen(listenWaitMaxValue);
            SocketAsyncEventArgs args = new();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }

        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            lock (lck)
            {
                if (args.SocketError != SocketError.Success)
                {
                    Console.WriteLine($"OnAcceptCompleted Error - {args.SocketError}");
                    return;
                }
                Session session = sessonFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            if (socket.AcceptAsync(args) == false)
                OnAcceptCompleted(null, args);
        }
    }
}
