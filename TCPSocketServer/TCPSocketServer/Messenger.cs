namespace TCPSocketServer
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    public static class Messenger
    {
        public static string Read(Client client)
        {
            byte[] buffer = new byte[1];
            int length = 0;
            StringBuilder strBuild = new StringBuilder();
            client.Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
            string character = Encoding.UTF8.GetString(buffer);

            if (character == "[")
            {
                while (true)
                {
                    client.Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
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
            client.Socket.Receive(message, 0, message.Length, SocketFlags.None);
            return Encoding.UTF8.GetString(message);
        }

        public static bool Send(Client client, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string toSend = string.Format("[{0}]{1}", message.Length, message);
                byte[] buffer = Encoding.UTF8.GetBytes(toSend);

                if (client.Socket.Connected)
                {
                    client.Socket.Send(buffer, buffer.Length, SocketFlags.None);
                    return true;
                }
            }
            return false;
        }
    }
}
