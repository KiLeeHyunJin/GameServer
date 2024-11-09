using System.Net;
using System.Net.Sockets;
using ServerCore;

namespace Server
{
    public class Lobby
    {
        List<GameRoom> emptyList = new List<GameRoom>(3);
        List<GameRoom> fullList = new List<GameRoom>(3);

        public void Flush()
        {
            for (int i = 0; i < emptyList.Count; i++)
            {
                emptyList[i].Push(() => { emptyList[i].Flush(); });
            }
            for (int i = 0; i < fullList.Count; i++)
            {
                fullList[i].Push(() => { fullList[i].Flush(); });
            }
        }

        public void RemoveRoom(GameRoom room)
        {
            fullList.Remove(room);
        }

        public void EnterLobby(ClientSession session)
        {
            GameRoom enterRoom;

            if (emptyList.Count > 0)
            {
                enterRoom = emptyList[0];
                emptyList.RemoveAt(0);
                fullList.Add(enterRoom);
            }
            else
            {
                enterRoom = new GameRoom();
                enterRoom.Lobby = this;
                emptyList.Add(enterRoom);
            }
            enterRoom.Enter(session);
        }
    }
}
