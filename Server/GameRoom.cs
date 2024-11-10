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
        struct UniCast
        {
            public ArraySegment<byte> _pending;
            public short _sendId;
        }

        List<ClientSession> _sessions = new(2);
        List<ArraySegment<byte>> _pendingList = new();
        List<UniCast> _unicastList = new(3);

        bool[] resultCheck = new bool[2];
        bool _ready = true;

        Job _queue = new();
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

        public void Unicast(ArraySegment<byte> segment, short targetId)
        {
            _unicastList.Add(new() { _pending = segment, _sendId = targetId });
        }

        public void Flush()
        {
            int count = _sessions.Count;
            if(count > 0)
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        _sessions[i].Send(_pendingList);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                _pendingList.Clear();
            }

            if(count > 1)
            {
                count = _unicastList.Count;
                if(count == 0)
                {
                    return;
                }

                ClientSession targetSession;
                ArraySegment<byte> pending;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        targetSession = _unicastList[i]._sendId == 0 ? _sessions[1] : _sessions[0];
                        pending = _unicastList[i]._pending;
                        targetSession.Send(pending);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Enter(ClientSession session)
        {
            session.SetRoom = this;
            session.SessionId = (short)_sessions.Count;

            S_BroadcastEnterGame enter = new()
            {
                playerId = session.SessionId
            };
            Broadcast(enter.Write());

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
        }

        public void LastBan(ClientSession session, C_BanPick packet)
        {
            S_LastBanPick p = new()
            {
                lastBanIdx = packet.banId
            };
            Unicast(p.Write(), session.SessionId);
        }

        public void Ban(ClientSession session, C_BanPick packet)
        {
            S_BanPick p = new()
            { 
                banId = packet.banId
            };
            Unicast(p.Write(), session.SessionId);
        }

        public void Pick(ClientSession session, C_PickUp packet)
        {
            S_PickUp p = new()
            {
                pickIdx = packet.pickIdx,
            };
            Unicast(p.Write(), session.SessionId);
        }

        public void Attack(ClientSession session, C_Attck packet)
        {
            S_Attck p = new()
            {
                atckId = packet.atckId,
                skillId = packet.skillId,
                damValue = packet.damValue
            };
            Unicast(p.Write(), session.SessionId);
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);

            S_BroadcastLeaveGame leaveGame = new()
            {
                playerId = (short)session.SessionId   
            };

            Broadcast(leaveGame.Write());
            if(_sessions.Count <= 1)
            {
                session.Disconnect();
                RemoveRoom();
            }
        }

        public void Chat(ClientSession session, C_Chat packet)
        {
            S_Chat c = new();
            c.playerId = session.SessionId;
            c.chat = packet.chat;
            Broadcast(c.Write());
        }

        public void Result(ClientSession session)
        {
            resultCheck[session.SessionId] = true;
            foreach (var item in resultCheck)
            {
                if(item == false)
                {
                    return;
                }
            }
            S_Result p = new();
            ArraySegment<byte> s = p.Write();
            Broadcast(s);
        }

    }
}
