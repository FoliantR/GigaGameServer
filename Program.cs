using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GigaGameServer
{
    class Program
    {
        static ServerObject server;
        static Thread listenThread;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                server.Disconnect();
            }
        }
    }
}