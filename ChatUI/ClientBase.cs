using System;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    public class ClientBase
    {
        System.Net.Sockets.TcpClient Client = new System.Net.Sockets.TcpClient();
        NetworkStream ServerNetStream;
        string readData;
        static string ChatContents = "";
        /// <summary>
        /// Sends message to the chat server.
        /// </summary>
        /// <param name="Message">Chat message to be sent.</param>
        private void SendMessage(string Message)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(Message + "$");
            ServerNetStream.Write(outStream, 0, outStream.Length);
            ServerNetStream.Flush();
        }

        private void UpdateChat()
        {
            ChatContents = ChatContents + Environment.NewLine + " >> " + readData;
        }

        private void GetServerMessages()
        {
            while (true)
            {
                ServerNetStream = Client.GetStream();
                int buffSize = Client.ReceiveBufferSize;
                byte[] inStream = new byte[buffSize];
                Console.WriteLine("BuffSize: {0}", buffSize);
                ServerNetStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;
                UpdateChat();
            }
        }

        private void ConnectServer(string ServerAddress, string UserName)
        {
            readData = "Connected to Chat Server ...";
            UpdateChat();
            Client.Connect(ServerAddress, 8888);
            ServerNetStream = Client.GetStream();

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(UserName + "$");
            ServerNetStream.Write(outStream, 0, outStream.Length);
            ServerNetStream.Flush();

            Thread ctThread = new Thread(GetServerMessages);
            ctThread.Start();
        }




    }
}
