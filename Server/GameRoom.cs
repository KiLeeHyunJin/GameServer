using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameRoom : IJob
    {
        class UniCast
        {
            public ArraySegment<byte> _pending;
            public ClientSession _target;
        }

        List<ClientSession> _sessions = new(2);
        List<ArraySegment<byte>> _pendingList = new();
        List<UniCast> _unicastList = new();

        Job _queue = new();
        bool _ready = true;
        public int RoomIndex { get; set; }
        public int PlayerCount { get { return _sessions.Count; } }
        public Lobby Lobby { get; set; }

        public void RemoveRoom()
        {
            foreach (var item in _sessions)
            {
                _queue.Push(() =>
                {
                    foreach (var s in _sessions)
                    {
                        s.Disconnect();
                    }
                });
            }
            Lobby.RemoveRoom(this, _ready);
        }

        public void Push(Action job)
        {
            _queue.Push(job);
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Unicast(ArraySegment<byte> segment, ClientSession session)
        {
            _unicastList.Add(new() { _pending = segment, _target = session });
        }

        public void Flush()
        {
            int count = _sessions.Count;
            if(count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    _sessions[i].Send(_pendingList);
                }
                _pendingList.Clear();
            }

            count = _unicastList.Count;
            if(count > 0)
            {
                ClientSession targetSession;
                ArraySegment<byte> pending;
                for (int i = 0; i < count; i++)
                {
                    targetSession = _unicastList[i]._target;
                    pending = _unicastList[i]._pending;
                    targetSession.Send(pending);
                }
                _unicastList.Clear();
            }
        }

        public void Enter(ClientSession session)
        {
            session.SetRoom = this;

            int count = _sessions.Count;
            session.SessionId = count;

            _sessions.Add(session);

            if(_sessions.Count > 1)
            {
                _ready = false;
            }

            S_PlayerList players = new();
            foreach (var s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = (short)s.SessionId
                });
            }
            session.Send(players.Write());

            S_BroadcastEnterGame enter = new()
            {
                playerId = (short)session.SessionId
            };

            Broadcast(enter.Write());
        }

        public void Ban(ClientSession session, C_BanPick packet)
        {
            S_BanPick p = new()
            { 
                banId = packet.banId
            };

        }

        public void Pick(ClientSession session, C_PickUp packet)
        {

        }

        public void Attack(ClientSession session, C_Attck packet)
        {

        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
            if(_ready == false)
            {
                RemoveRoom();
            }

            S_BroadcastLeaveGame leaveGame = new()
            {
                playerId = (short)session.SessionId   
            };

            Broadcast(leaveGame.Write());
        }

        public void Chat(ClientSession session, C_Chat packet)
        {
            S_Chat c = new();
            c.playerId = session.SessionId;
            c.chat = packet.chat;
            Broadcast(c.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            session.PosX = packet.posX;
            session.PosY = packet.posX;
            session.PosZ = packet.posZ;

            S_BroadcastMove move = new()
            {
                playerId = (short)session.SessionId
            };

            Broadcast(move.Write());
        }


    }
}
