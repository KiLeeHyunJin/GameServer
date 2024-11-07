using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    internal class SessionManager
    {
        public static SessionManager Instance { get { return _session; } }
        static SessionManager _session = new();
        List<ServerSession> _sessions = new();
        object _lock = new();

        public void SendForEach()
        {
            lock(_lock)
            {
                foreach (var session in _sessions)
                {
                    C_Chat p = new();
                    p.chat = $"Hellow Server!";
                    ArraySegment<byte> segment = p.Write();
                    session.Send(segment);
                }
            }
        }

        public ServerSession Generate()
        {
            lock(_lock)
            {
                ServerSession session = new();
                _sessions.Add(session);
                return session;
            }
        }



    }
}
