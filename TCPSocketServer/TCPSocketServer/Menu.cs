namespace TCPSocketServer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Menu
    {
        private const string exitKey = "<exit>";
        private static string separater = new string('-', 60);
        private string mainOptions = separater + "\n" +
            "Main Menu (Type the number of the option you want):\n" +
            "[1]Send private message.\n" +
            "[2]Create chat room.\n" +
            "[3]Join chat room.\n" +
            "(Note: To exit any of the options type \"" + exitKey + "\")\n" +
            separater;
        private string userExistenceError = "User doesn't exist! Try again.";
        private Client client;
        private List<Client> clients;
        private List<ChatRoom> chatRooms;

        public Menu(Client client, List<Client> clients, List<ChatRoom> chatRooms)
        {
            this.Client = client;
            this.clients = clients;
            this.chatRooms = chatRooms;
        }

        public string MainOptions
        {
            get
            {
                return this.mainOptions;
            }
            private set
            {
                this.mainOptions = value;
            }
        }

        public string UserExistenceError
        {
            get
            {
                return this.userExistenceError;
            }
            private set
            {
                this.userExistenceError = value;
            }
        }

        public Client Client
        {
            get
            {
                return this.client;
            }
            private set
            {
                this.client = value;
            }
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

        public void Entry()
        {
            Messenger.Send(this.Client, this.MainOptions);
            string choice = string.Empty;

            do
            {
                choice = Messenger.Read(this.Client);

                if (!string.IsNullOrEmpty(choice))
                {
                    switch (choice)
                    {
                        case "1":
                            SendPrivateMessage();
                            break;
                        case "2":
                            CreateChatRoom();
                            break;
                        case "3":
                            JoinChatRoom();
                            break;
                        default:
                            Messenger.Send(this.Client, "Option doesn't exist! Try Again.");
                            continue;
                    }
                    break;
                }
            }
            while (true);
        }

        private void SendPrivateMessage()
        {
            string availableClients = GetAllClientsAsString();

            if(string.IsNullOrEmpty(availableClients))
            {
                Messenger.Send(this.Client, "There is nobody online");
            }
            else
            {
                Messenger.Send(this.Client, "Who do you want to chat with?\n" + availableClients);
                string username = string.Empty;
                Client receiver = null;
                string message = string.Empty;

                do
                {
                    username = Messenger.Read(this.Client);
                    receiver = FindClientByUsername(username);

                    if (receiver != null)
                    {

                        break;
                    }

                    Messenger.Send(this.Client, this.UserExistenceError);
                }
                while (true);

                Messenger.Send(this.Client, "You can start typing:");

                while (true)
                {
                    message = Messenger.Read(this.Client).Trim();

                    if (message == exitKey)
                    {
                        break;
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        if (!Messenger.Send(receiver, DateTime.Now.ToString() + "| " + this.Client.Username + ": " + message))
                        {
                            Messenger.Send(this.Client, receiver.Username + " is no longer available");
                            this.Clients.Remove(receiver);
                            break;
                        }
                    }
                }
            }

            Entry();
        }

        private void CreateChatRoom()
        {
            string additionExitKey = "<enough>";
            Messenger.Send(this.Client, "Name of the chat room:");
            string nameOfChatRoom = string.Empty;

            do
            {
                nameOfChatRoom = Messenger.Read(this.Client);

                if (string.IsNullOrEmpty(nameOfChatRoom))
                {
                    continue;
                }
                else if (CheckChatRoomExistence(nameOfChatRoom))
                {
                    Messenger.Send(this.Client, "Chat room already exists! Try again.");
                    continue;
                }

                break;
            }
            while (true);

            Messenger.Send(this.Client, "Type all the people you want to chat with (each on a new line):\n(Note: type \"" + additionExitKey + "\" to stop adding.)\n" + GetAllClientsAsString());
            ChatRoom chatRoom = new ChatRoom(nameOfChatRoom);
            chatRoom.AddMember(this.Client);
            this.ChatRooms.Add(chatRoom);
            string input = string.Empty;
            Client newMember = null;

            while (true)
            {
                input = Messenger.Read(this.Client);

                if (input == additionExitKey)
                {
                    break;
                }

                newMember = FindClientByUsername(input);

                if (newMember != null)
                {
                    chatRoom.AddMember(newMember);
                    newMember = null;
                }
                else
                {
                    Messenger.Send(this.Client, this.UserExistenceError);
                }
            }

            Messenger.Send(this.Client, "Chatroom created. You can start typing:");
            chatRoom.Chat(this.Client, exitKey);
            Entry();
        }

        private void JoinChatRoom()
        {
            string availableChatRooms = FindChatRoomsForMember();

            if (string.IsNullOrEmpty(availableChatRooms))
            {
                Messenger.Send(this.Client, "There are no available chat rooms");
            }
            else
            {
                Messenger.Send(this.Client, "Where do you want to join?\n" + availableChatRooms);
                string roomName = string.Empty;
                ChatRoom room = null;
                string message = string.Empty;

                do
                {
                    roomName = Messenger.Read(this.Client);
                    room = FindChatRoomByName(roomName);

                    if (room != null)
                    {
                        break;
                    }

                    Messenger.Send(this.Client, "Chat room doesn't exist! Try again.");
                }
                while (true);

                Messenger.Send(this.Client, "Chat room joined! You can start typing:");
                room.Chat(this.Client, exitKey);
            }

            Entry();
        }

        private string GetAllClientsAsString()
        {
            StringBuilder strBuild = new StringBuilder();

            foreach (Client c in this.Clients)
            {
                if (c != this.Client)
                {
                    strBuild.Append($"- {c.Username}\n");
                }
            }

            return strBuild.ToString();
        }

        private Client FindClientByUsername(string username)
        {
            Client client = null;

            foreach (Client c in this.Clients)
            {
                if (c.Username == username)
                {
                    client = c;
                    break;
                }
            }

            return client;
        }

        private string FindChatRoomsForMember()
        {
            StringBuilder strBuild = new StringBuilder();

            foreach (ChatRoom cr in this.ChatRooms)
            {
                foreach (Client c in cr.Members)
                {
                    if (c == this.Client)
                    {
                        strBuild.Append($"- {cr.Name}\n");
                        break;
                    }
                }
            }

            return strBuild.ToString();
        }

        private ChatRoom FindChatRoomByName(string roomName)
        {
            ChatRoom room = null;

            foreach (ChatRoom cr in this.ChatRooms)
            {
                if (cr.Name == roomName)
                {
                    room = cr;
                    break;
                }
            }

            return room;
        }

        private bool CheckChatRoomExistence(string name)
        {
            foreach (ChatRoom c in this.ChatRooms)
            {
                if (c.Name == name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}