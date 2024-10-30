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
        List<Room> emptyList = new List<Room>(3);
        List<Room> fullList = new List<Room>(3);

        public void EnterLobby(Socket enterSocket, EndPoint endPoint)
        {
            for (int i = 0; i < fullList.Count; i++)
            {
                if (fullList[i].Finish)
                {
                    fullList.RemoveAt(i);
                }
            }

            if (emptyList.Count > 0)
            {
                Room enterRoom = emptyList[0];

                enterRoom.Start(enterSocket);
                enterRoom.OnConnected(endPoint);

                emptyList.RemoveAt(0);
                fullList.Add(enterRoom);
            }
            else
            {
                emptyList.Add(new Room());
            }

        }

    }
}
