using ServerCore;

namespace Server
{
    internal class Program
    {
        static Listener _listener = new();
        readonly public static GameRoom Room = new();
        readonly public static Lobby Lobby = new();
        static void FlushRoom()
        {
            Lobby.Flush();
            //Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 1000);
        }

        static void Main(string[] args)
        {
            try
            {
                _listener.Init(() => { return SessionManager.Instance.Generate(); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("Listening...");

            JobTimer.Instance.Push(FlushRoom);

            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }

}
