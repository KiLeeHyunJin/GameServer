using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class GameRoom : IJob
    {
        List<ClientSession> _sessions = new(2);
        Job _queue = new();

        bool isFinish = false;

        public void Push(Action job)
        {
            _queue.Push(job);

        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat p = new S_Chat() {   playerId = session.SessionId    };
            p.chat = $"{chat} - i am {session.SessionId}";
            ArraySegment<byte> segment = p.Write();

            for (int i = 0; i < _sessions.Count; i++)
            {
                _sessions[i].Send(segment);
            }
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
            if (isFinish == false && _sessions.Count > 0)
            {
                S_Result result = new();
                result.result = true;
                _sessions[0].Send(result.Write());
            }
        }


    }
}
