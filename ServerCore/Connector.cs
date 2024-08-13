using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            Socket socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
            arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectComplete);
            arg.RemoteEndPoint = endPoint;
            arg.UserToken = socket;
        }

        public void RegisterConnect(SocketAsyncEventArgs arg)
        {
            Socket socket = arg.UserToken as Socket;
            if (socket == null)
            {
                return;
            }
            bool panding = socket.ConnectAsync(arg);
            if (panding == false)
            {
                OnConnectComplete(null, arg);
            }
        }

        public void OnConnectComplete(object sender, SocketAsyncEventArgs arg)
        {
            if (arg.SocketError == SocketError.SocketError)
            {
                Console.WriteLine($"OnConnectComplete Error : {arg.SocketError}");
                return;
            }
            Session session = sessionFactory.Invoke();
            session.Start(arg.ConnectSocket);
            session.OnConnected(arg.RemoteEndPoint);
        }
    }
}
