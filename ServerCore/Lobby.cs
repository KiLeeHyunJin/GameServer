using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Lobby
    {
        List<BattleRoom> emptyList = new List<BattleRoom>(3);
        List<BattleRoom> fullList = new List<BattleRoom>(3);

        public Session EnterLobby(Socket enterSocket, EndPoint endPoint)
        {
            BattleRoom enterRoom = null;
            for (int i = 0; i < fullList.Count; i++)
            {
                if (fullList[i].Finish)
                {
                    fullList.RemoveAt(i);
                }
            }

            if (emptyList.Count > 0)
            {
                enterRoom = emptyList[0];

                enterRoom.Start(enterSocket);
                enterRoom.OnConnected(endPoint);

                emptyList.RemoveAt(0);
                fullList.Add(enterRoom);
            }
            else
            {
                enterRoom = new BattleRoom();
                emptyList.Add(enterRoom);
            }
            return enterRoom;
        }

    }
}
