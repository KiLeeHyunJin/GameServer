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
        int readyCount;
        int battleCount;
        public void Flush()
        {
            bool change = false;
            if(readyCount != readyList.Count)
            {
                readyCount = readyList.Count;
                change = true;
            }
            if(battleCount != battleList.Count)
            {
                battleCount = battleList.Count;
                change = true;
            }
            if(change)
            {
                Console.WriteLine(
                    $"ReadyRoom Count : {readyList.Count} " +
                    $"BattleRoom Count : {battleList.Count}");
            }

            try
            {
                for (int i = 0; i < readyList.Count; i++)
                {
                    GameRoom room = readyList[i];
                    room.Push(() => { room.Flush(); });
                }
                for (int i = 0; i < battleList.Count; i++)
                {
                    GameRoom room = battleList[i];
                    room.Push(() => { room.Flush(); });
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void RemoveRoom(GameRoom room, bool readyRoom)
        {
            List<GameRoom> roomList = readyRoom ? readyList : battleList;
            roomList.Remove(room);
        }

        public void EnterLobby(ClientSession session)
        {
            GameRoom enterRoom;
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
