﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class SessionManager
    {
        static SessionManager _session = new();
        public static SessionManager Instance { get { return _session; } }

        short _sessionId = 0;

        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();

        public ClientSession Generate()
        {
            lock(_lock)
            {
                if(_sessionId == short.MaxValue)
                {
                    _sessionId = 0;
                }
                short sessionId = ++_sessionId;
                ClientSession session = new()
                {
                    SessionId = sessionId
                };
                _sessions.Add(sessionId, session);
                return session;
            }
        }

        public ClientSession Find(int id)
        {
            lock(_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock(_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }

    }
}
