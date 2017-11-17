namespace TCPSocketServer
{
    using System;

    class EntryPoint
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Socket server");
            Server server = new Server();
            server.StartListening();
        }
    }
}
