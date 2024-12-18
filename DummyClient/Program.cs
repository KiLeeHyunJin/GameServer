﻿using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{

    internal class Program
    {

        static void Main(string[] args)
        {
            ServerSession session = new();
            Define.Connect connect = Define.Connect.Domain;

            string domain = "pkc-5000.shop";
            string local = Dns.GetHostName();
            
            string connectStr = connect == Define.Connect.Local ? local : domain;
            IPAddress[] addresses = Dns.GetHostAddresses(connectStr);

            Thread.Sleep(1000);

            try
            {
                foreach (var address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPEndPoint remoteEndPoint = new IPEndPoint(address, ServerCore.Define.PortNum);
                        Console.WriteLine($"[RemoteAddress] : {remoteEndPoint} ");

                        Connector connector = new Connector();
                        connector.Connect(
                            remoteEndPoint, 
                            () => { return session; },
                            connect,
                            1);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); 
            }


            while (true) 
            {
                try
                {
                    SessionManager.Instance.SendForEach(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(1500);
                string input = Console.ReadLine();
                if(int.TryParse(input, out int temp))
                {
                    session.Disconnect();
                }
            }
        }
    }

}
