namespace TCPSocketServer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class Server
    {
        private List<Client> clients;
        private List<ChatRoom> chatRooms;

        public Server()
        {
            this.clients = new List<Client>();
            this.chatRooms = new List<ChatRoom>();
        }

        public List<Client> Clients
        {
            get
            {
                return this.clients;
            }
        }

        public List<ChatRoom> ChatRooms
        {
            get
            {
                return this.chatRooms;
            }
        }

        public void AddClient(Client client)
        {
            this.Clients.Add(client);
        }

        public void RemoveClient(Client client)
        {
            this.Clients.Remove(client);
        }

        public void StartListening()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                IPEndPoint localEndPoint = new IPEndPoint(localAddr, 8888);

                Socket listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);
                Console.WriteLine("Waiting for a connection...");

                while (true)
                {
                    Socket handler = listener.Accept();
                    Console.WriteLine("A socket is accepted!");
                    Thread socketHandling = new Thread(HandleSocket);
                    socketHandling.Start(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleSocket(object tag)
        {
            try
            {
                var socket = (Socket)tag;
                Guid guid = Guid.NewGuid();
                Client client = new Client(socket, guid);
                Messenger.Send(client, guid.ToString());
                Messenger.Send(client, "Please input username:");
                string username = string.Empty;

                do
                {
                    username = Messenger.Read(client);

                    if(string.IsNullOrEmpty(username))
                    {
                        continue;
                    }
                    else if (CheckUsernameExistence(username))
                    {
                        Messenger.Send(client, "Username already exists! Try again.");
                        continue;
                    }

                    break;
                }
                while (true);

                client.Username = username;
                AddClient(client);
                Messenger.Send(client, "Your username is: " + client.Username);
                Menu menu = new Menu(client, this.Clients, this.ChatRooms);
                menu.Entry();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool CheckUsernameExistence(string username)
        {
            foreach (Client c in this.Clients)
            {
                if(c.Username == username)
                {
                    return true;
                }
            }

            return false;
        }
    }
}