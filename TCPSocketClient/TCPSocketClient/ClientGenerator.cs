using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPSocketClient
{
    public class ClientGenerator
    {
        public void GenerateClient()
        {
            try
            {
                int port = 8888;
                string host = "127.0.0.1";
                TcpClient client = new TcpClient(host, port);
                NetworkStream stream = client.GetStream();

                string guid = ReadMesssage(stream);
                string usernameRequest = ReadMesssage(stream);
                Console.WriteLine(usernameRequest);
                string username = Console.ReadLine();
                SendMessage(stream, username);
                Task.Run(() => Reading(stream));
                string message = string.Empty;

                while (true)
                {
                    message = Console.ReadLine();
                    SendMessage(stream, message);
                    
                    if(message == "quit")
                    {
                        break;
                    }
                }

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Reading(NetworkStream stream)
        {
            while (true)
            {
                Console.WriteLine(ReadMesssage(stream));
            }
        }

        private string ReadMesssage(NetworkStream stream)
        {
            byte[] buffer = new byte[10];
            stream.Read(buffer, 0, buffer.Length);
            string lengthString = Encoding.UTF8.GetString(buffer);
            bool successfulParse = uint.TryParse(lengthString, out uint length);

            if (!successfulParse)
            {
                throw new ArgumentException("Message is not in correct format!");
            }

            byte[] message = new byte[length];
            stream.Read(message, 0, message.Length);
            return Encoding.UTF8.GetString(message);
        }

        private void SendMessage(NetworkStream stream, string message)
        {
            string toSend = string.Format("{0}{1}", message.Length.ToString("#0000000000"), message);
            byte[] buffer = Encoding.UTF8.GetBytes(toSend);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
