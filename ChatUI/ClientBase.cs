using System;
using System.Net.Sockets;
using System.Threading;

namespace ChatUI
{
    // Partially sourced from: http://csharp.net-informations.com/communications/csharp-chat-server.htm
    public class ClientBase
    {
        System.Net.Sockets.TcpClient Client = new System.Net.Sockets.TcpClient();
        NetworkStream ServerNetStream;
        Thread ctThread;
        private string readData;
        private string fromUser;
        private string newMessage;
        private MainWindow ParentChatWindow;
        private EncryptionHandler encHandler = new EncryptionHandler();

        /// <summary>
        /// Sends message to the chat server.
        /// </summary>
        /// <param name="Message">Chat message to be sent.</param>
        public void SendMessage(string Message)
        {
            string encryptedMessage = encHandler.Encrypt(Message);
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(encryptedMessage + "$");
            ServerNetStream.Write(outStream, 0, outStream.Length);
            ServerNetStream.Flush();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ChatUI.ClientBase"/> class.
        /// </summary>
        /// <param name="parentChatWindow">Parent chat window, used to update chat field.</param>
        public ClientBase(MainWindow parentChatWindow)
        {
            this.ParentChatWindow = parentChatWindow;
        }

        /// <summary>
        /// Updates the chat contents on the parent UI screen.
        /// </summary>
        private void UpdateChat()
        {
            ParentChatWindow.AddChatMessage("<" + fromUser + ">: " + newMessage);
        }


        private void GetServerMessages()
        {
            string[] parsedData;
            while (true)
            {
                ServerNetStream = Client.GetStream();
                int bufferSize = Client.ReceiveBufferSize;
                byte[] inputStream = new byte[bufferSize];
                ServerNetStream.Read(inputStream, 0, bufferSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inputStream);
                readData = "" + returndata;
                if(readData.Contains("|"))
                {
                    parsedData = readData.Split('|');
                    Console.WriteLine(parsedData[1].Remove(0, parsedData[1].Length));
                    // Decrypt username and message
                    fromUser = encHandler.Decrypt(parsedData[0]);
                    // Removes extra data after the == in the encrypted message, which causes crashing.
                    //newMessage = encHandler.Decrypt(parsedData[1]);
                    newMessage = encHandler.Decrypt(parsedData[1].Remove(parsedData[1].IndexOf('$')));
                    //newMessage = encHandler.Decrypt(parsedData[1].Remove(parsedData[1].IndexOf('=') + 2));
                    //newMessage = parsedData[1].Remove(parsedData[1].IndexOf('=') + 2);
                    UpdateChat();
                    //ParentChatWindow.AddChatMessage(readData);
                }
                else if(readData.StartsWith("j:"))
                {
                    newMessage = readData.Remove(0, 2);
                    newMessage = newMessage.Remove(newMessage.IndexOf('$'));
                    newMessage = encHandler.Decrypt(newMessage) + " joined";
                    //newMessage
                    //newMessage = newMessage.Remove(newMessage.IndexOf('=') + 2);
                    ParentChatWindow.AddChatMessage(newMessage);
                    //UpdateChat();
                    //newMessage = encHandler.Decrypt(readData.Remove(0, 2).Remove(readData.IndexOf('=') + 2));
                }
                else
                {
                    ParentChatWindow.AddChatMessage(readData);
                }
            }
        }

        public void ConnectToServer(string serverAddress, string userName, string password)
        {
            string encryptedUserName;
            ParentChatWindow.AddChatMessage("Connected to Chat Server ...");
            encHandler.SetKey(password);
            //UpdateChat();
            Client.Connect(serverAddress, 8888);
            ServerNetStream = Client.GetStream();
            encryptedUserName = encHandler.Encrypt(userName);
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(encryptedUserName + "$");
            ServerNetStream.Write(outStream, 0, outStream.Length);
            ServerNetStream.Flush();

            ctThread = new Thread(GetServerMessages);
            ctThread.Start();
        }

        /// <summary>
        /// Stops the client thread, called on application exit.
        /// </summary>
        public void StopClient()
        {
            ctThread.Abort();
        }
    }
}
