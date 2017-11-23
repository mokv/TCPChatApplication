namespace TCPSocketServer
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    public static class Messenger
    {
        public static string Read(Client client)
        {
            byte[] buffer = new byte[10];
            client.Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
            string lengthString = Encoding.UTF8.GetString(buffer);
            bool successfulParse = uint.TryParse(lengthString, out uint length);

            if (!successfulParse)
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
                string toSend = string.Format("{0}{1}", message.Length.ToString("#0000000000"), message);
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
