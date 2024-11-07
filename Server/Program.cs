using ServerCore;

namespace Server
{
    internal class Program
    {
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            Listener _listener = new Listener();
            try
            {
                _listener.Init(() => { return SessionManager.Instance.Generate(); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }

}
