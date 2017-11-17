using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPSocketClient
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Socket Client");
            ClientGenerator clientGenerator = new ClientGenerator();
            clientGenerator.GenerateClient();
        }
    }
}
