using ChatServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    // Partially sourced from: http://csharp.net-informations.com/communications/csharp-chat-server.htm
    public class ClientHandler
    {
        TcpClient clientSocket;
        string clientUserName;
        Hashtable clientsList;

        public void StartHandler(TcpClient inClientSocket, string clineNo, Hashtable cList)
        {
            this.clientSocket = inClientSocket;
            this.clientUserName = clineNo;
            this.clientsList = cList;
            Thread ctThread = new Thread(RecieveChatLoop);
            ctThread.Start();
        }

        private void RecieveChatLoop()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
            string dataFromClient = null;
            //Byte[] sendBytes = null;
            //string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while (true)
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine("From client - " + clientUserName + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    Program.SendBroadCast(dataFromClient, clientUserName, true);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                }
            }
        }
    } 
}
