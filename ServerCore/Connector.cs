using System.Net;
using System.Net.Sockets;


namespace ServerCore
{
    public class Connector
    {
        Func<Session> _sessionFactory;
        Lobby lobby = new Lobby();

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _sessionFactory = sessionFactory;

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = 30;
            
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.UserToken = socket;
            args.RemoteEndPoint = endPoint;
            args.Completed += OnConnectedCompleted;

            RegistConnect(args);
        }

        void RegistConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
            {
                return;
            }
            bool pending = socket.ConnectAsync(args);
            if(pending == false)
            {
                OnConnectedCompleted(null, args);
            }
        }

        void OnConnectedCompleted(object? sender, SocketAsyncEventArgs args)
        {
            Console.WriteLine("OnConnectedCompleted");
            if(args.SocketError == SocketError.Success)
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