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
                Console.WriteLine(ReadMesssage(stream));
                SendMessage(stream, Console.ReadLine());
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
            byte[] buffer = new byte[1];
            int length = 0;
            StringBuilder strBuild = new StringBuilder();
            stream.Read(buffer, 0, buffer.Length);
            string character = Encoding.UTF8.GetString(buffer);

            if (character == "[")
            {
                while (true)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    character = Encoding.UTF8.GetString(buffer);

                    if (character == "]")
                    {
                        length = int.Parse(strBuild.ToString());
                        break;
                    }
                    else
                    {
                        strBuild.Append(character);
                    }
                }
            }
            else
            {
                throw new ArgumentException("Message is not in correct format!");
            }

            byte[] message = new byte[length];
            stream.Read(message, 0, message.Length);
            return Encoding.UTF8.GetString(message);
        }

        private void SendMessage(NetworkStream stream, string message)
        {
            string toSend = string.Format("[{0}]{1}", message.Length, message);
            byte[] buffer = Encoding.UTF8.GetBytes(toSend);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
