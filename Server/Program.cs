using ServerCore;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Listener _listener = new Listener();
            try
            {
                _listener.Init(() => { return new ClientSession(); });
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
