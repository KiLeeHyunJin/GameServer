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

        public void Flush()
        {
            for (int i = 0; i < readyList.Count; i++)
            {
                readyList[i].Push(() => { readyList[i].Flush(); });
            }
            for (int i = 0; i < battleList.Count; i++)
            {
                battleList[i].Push(() => { battleList[i].Flush(); });
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
