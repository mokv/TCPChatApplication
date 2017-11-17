namespace TCPSocketServer
{
    using System;
    using System.Collections.Generic;

    public class ChatRoom
    {
        private string name;
        private readonly Guid guid;
        private List<Client> members;

        public ChatRoom(string name)
        {
            this.Name = name;
            this.guid = new Guid();
            this.members = new List<Client>();
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public Guid Guid
        {
            get
            {
                return this.guid;
            }
        }

        public List<Client> Members
        {
            get
            {
                return this.members;
            }
        }

        public void AddMember(Client member)
        {
            this.Members.Add(member);
        }

        public void RemoveMember(Client member)
        {
            this.Members.Remove(member);
        }

        public void Chat(Client sender, string exitKey)
        {
            List<Client> toRemove = new List<Client>();
            string message = string.Empty;
            bool needsRemoval = false;

            while (true)
            {
                message = Messenger.Read(sender).Trim();

                if (message == exitKey)
                {
                    break;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    foreach (Client m in this.Members)
                    {
                        if (m != sender)
                        {
                            if (!Messenger.Send(m, DateTime.Now.ToString() + "| " + this.Name + " | " + sender.Username + ": " + message))
                            {
                                Messenger.Send(sender, m.Username + " did not receive the message!");
                                toRemove.Add(m);
                                needsRemoval = true;
                            }
                        }
                    }
                }

                if (needsRemoval)
                {
                    foreach(Client m in toRemove)
                    {
                        this.Members.Remove(m);
                        SendToAll(m.Username + " left the conversation.");
                    }

                    needsRemoval = false;
                    toRemove.Clear();
                }
            }
        }

        public void SendToAll(string message)
        {
            foreach(Client member in this.Members)
            {
                Messenger.Send(member, message);
            }
        }
    }
}
