using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace ChatServer
{
    class Program
    {
        static Hashtable clientsList = new Hashtable();
        static int buffSize = 0;
        static bool running = true;

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Chat Server Started ....");
            counter = 0;
            while (running)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                buffSize = clientSocket.ReceiveBufferSize;
                byte[] bytesFrom = new byte[buffSize];
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, buffSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientsList.Add(dataFromClient, clientSocket);

                // Join message
                SendBroadCast("j:" + dataFromClient + "$", dataFromClient, false);

                Console.WriteLine(dataFromClient + " joined chat room ");
                ClientHandler client = new ClientHandler();
                client.StartHandler(clientSocket, dataFromClient, clientsList);
            }
            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void SendBroadCast(string message, string userName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(userName + "|" + message + "$");
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(message);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        } 
    }
}