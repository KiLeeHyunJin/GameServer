namespace DummyClient
{
    internal class SessionManager
    {
        public static SessionManager Instance { get { return _session; } }
        static SessionManager _session = new();
        List<ServerSession> _sessions = new();
        object _lock = new();
        Random _rand = new();

        public void SendForEach(bool  state)
        {
            if(state)
            {
                lock (_lock)
                {
                    foreach (var session in _sessions)
                    {
                        C_Chat c = new()
                        {
                            chat = $"Hellow Server!"
                        };
                        ArraySegment<byte> s = c.Write();
                        session.Send(s);
                    }
                }
            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new();
                _sessions.Add(session);
                return session;
            }
        }



    }
}
