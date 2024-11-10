using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ServerCore;

namespace Server
{
    public class Lobby
    {
        List<GameRoom> readyList = new List<GameRoom>(3);
        List<GameRoom> battleList = new List<GameRoom>(3);
        object _lock = new();
        public void Flush()
        {
            lock(_lock)
            {
                int ready = 0;
                int battle = 0;
                try
                {
                    for (int i = 0; i < readyList.Count; i++)
                    {
                        ready++;
                        GameRoom room = readyList[i];
                        room.Push(() => { room.Flush(); });
                    }
                    for (int i = 0; i < battleList.Count; i++)
                    {
                        battle++;
                        GameRoom room = battleList[i];
                        room.Push(() => { room.Flush(); });
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                }
                
            }
        }

        public void RemoveRoom(GameRoom room, bool readyRoom)
        {
            lock(_lock)
            {
                List<GameRoom> roomList = readyRoom ? readyList : battleList;
                roomList.Remove(room);
            }
        }

        public void EnterLobby(ClientSession session)
        {
            GameRoom enterRoom;
            lock(_lock)
            {
                if (readyList.Count > 0)
                {
                    enterRoom = readyList[0];
                    readyList.RemoveAt(0);
                    battleList.Add(enterRoom);
                }
                else
                {
                    enterRoom = new GameRoom();
                    enterRoom.Lobby = this;
                    readyList.Add(enterRoom);
                }
                enterRoom.Enter(session);
            }
           
        }
    }
}
