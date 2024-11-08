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
        List<ArraySegment<byte>> _pendingList = new();

        Job _queue = new();

        bool isFinish = false;

        public void Push(Action job)
        {
            _queue.Push(job);
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Flush()
        {
            foreach (var s in _sessions)
            {
                s.Send(_pendingList);
            }
            //Console.WriteLine($"Flush {_sessions.Count} Item");
            _pendingList.Clear();

        }

        public void Enter(ClientSession session)
        {
            //Console.WriteLine($"GameRoom Enter : {session.SessionId}");

            session.SetRoom = this;
            _sessions.Add(session);

            S_PlayerList players = new();
            foreach (var s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                { 
                    isSelf = (s == session),
                    playerId = (short)s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ
                });
            }
            session.Send(players.Write());

            S_BroadcastEnterGame enter = new()
            {
                playerId = (short)session.SessionId,
                posX = 0,
                posY = 0,
                posZ = 0
            };

            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);

            S_BroadcastLeaveGame leaveGame = new()
            {
                playerId = (short)session.SessionId   
            };

            Broadcast(leaveGame.Write());
            //if (isFinish == false && _sessions.Count > 0)
            //{
            //    S_Result result = new();
            //    result.result = true;
            //    _sessions[0].Send(result.Write());
            //}
        }

        public void Move(ClientSession session, C_Move packet)
        {
            session.PosX = packet.posX;
            session.PosY = packet.posX;
            session.PosZ = packet.posZ;

            S_BroadcastMove move = new()
            {
                playerId = (short)session.SessionId,
                posX = session.PosX,
                posY = session.PosY,
                posZ = session.PosZ
            };

            Broadcast(move.Write());
        }


    }
}
