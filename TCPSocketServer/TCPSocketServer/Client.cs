namespace TCPSocketServer
{
    using System;
    using System.Net.Sockets;

    public class Client
    {
        private readonly Socket socket;
        private readonly Guid guid;
        private string username;

        public Client(Socket socket, Guid guid)
        {
            this.socket = socket;
            this.guid = guid;
        }

        public Client(Socket socket, Guid guid, string nickname)
            :this(socket, guid)
        {
            this.Username = nickname;
        }

        public Socket Socket
        {
            get
            {
                return this.socket;
            }
        }

        public Guid Guid
        {
            get
            {
                return this.guid;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
            set
            {
                if(value.Length < 1)
                {
                    throw new ArgumentNullException("Nickname is too short!");
                }

                this.username = value;
            }
        }
    }
}
