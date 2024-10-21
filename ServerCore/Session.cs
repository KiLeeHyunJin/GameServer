using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class Session
    {
        const int recvBufferMaxSize = 1024;
        
        protected Socket socket;
        int disconnectState = default;

        RecBuffer _recBuffer = new(recvBufferMaxSize); 

        readonly object lck = new();
        readonly Queue<byte[]> sendQueue = new();
        readonly List<ArraySegment<byte>> sendPendingBufList = new();

        readonly SocketAsyncEventArgs sendArg = new();
        readonly SocketAsyncEventArgs recvArg = new();

        EventHandler<SocketAsyncEventArgs> recvEventHandler;
        EventHandler<SocketAsyncEventArgs> sendEventHandler;

        //byte[] recvBuff;

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buff);
        public abstract void OnSend(int byteSize);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            this.socket = socket;

            recvArg.Completed -= recvEventHandler;
            sendArg.Completed -= sendEventHandler;

            recvEventHandler = new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            sendEventHandler = new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            recvArg.Completed += recvEventHandler;
            sendArg.Completed += sendEventHandler;

            //if (recvBuff == null)
            //{
            //    recvBuff = new byte[recvBufferMaxSize];
            //    recvArg.SetBuffer(recvBuff, 0, recvBufferMaxSize);
            //}

            RegisterRecv();
        }




        public void Send(byte[] sendByte)
        {
            lock (lck)
            {
                sendQueue.Enqueue(sendByte);
                if (sendArg.BufferList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnectState, 1) != 0)
                return;
            OnDisconnected(socket.RemoteEndPoint);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }




        void RegisterSend()
        {
            sendPendingBufList.Clear();
            while (sendQueue.Count > 0)
            {
                byte[] sendBuff = sendQueue.Dequeue();
                sendPendingBufList.Add(new ArraySegment<byte>(sendBuff, 0, sendBuff.Length));
            }
            sendArg.BufferList = sendPendingBufList;

            bool pending = socket.SendAsync(sendArg);
            if (pending == false)
            {
                OnSendCompleted(null, sendArg);
            }
        }

        void RegisterRecv()
        {
            _recBuffer.Clear();
            ArraySegment<byte> segment = _recBuffer.WriteSegment;
            recvArg.SetBuffer(segment.Array, segment.Offset, segment.Count);
            bool pending = socket.SendAsync(recvArg);
            if (pending == false)
            {
                OnRecvCompleted(null, recvArg);
            }
        }


        void OnSendCompleted(object? sender, SocketAsyncEventArgs args)
        {
            lock (lck)
            {
                if (SocketAsyncEventErrorCheck(args) == false)
                {
                    try
                    {
                        if(_recBuffer.OnWrite(args.BytesTransferred) == false)
                        {
                            Disconnect();
                            return;
                        }
                        int progressLen = OnRecv(_recBuffer.WriteSegment);
                        if (progressLen < 0 || _recBuffer.DataSize < progressLen)
                        {
                            Disconnect();
                            return;
                        }
                        RegisterSend();

                        if(_recBuffer.OnRead(progressLen) == false)
                        {
                            Disconnect();
                            return;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"RecvCompleted Error : {e}");
                    }
                }
            }
        }

        void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (SocketAsyncEventErrorCheck(args) == false)
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"RecvCompleted Error : {e}");
                }
            }
        }

        bool SocketAsyncEventErrorCheck(SocketAsyncEventArgs args)
        {
            bool state =
                args.BytesTransferred <= 0 &&
                args.SocketError != SocketError.Success;
            if (state)
            {
                Console.WriteLine($"OnRecvCompleted Error - {args.SocketError}");
                Disconnect();
            }
            return state;
        }
    }
}
