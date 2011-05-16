using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Spacestro.Entities;

namespace Spacestro.Server
{    
    
    public class Server
    {
        // TODO: Determine our actual max message size.
        private const int MESSAGE_SIZE = 4096;

        // TODO: Find out if this thing is actually thread safe, the example was handling it like this.
        private TcpListener tcpListener; 

        public Server ()
	    {
            // TODO: read the port number from a config file.
            // TODO: confirm that localhost is in face what we should be listening to.
            tcpListener = new TcpListener(Dns.GetHostEntry("localhost").AddressList[0], 8383);            
            Thread listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
	    }

        /// <summary>
        /// Loops and listens for clients to connect. Spawns a thread to handle each client that does connect.
        /// </summary>
        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {                
                // Blocks until a client connects.
                TcpClient client = this.tcpListener.AcceptTcpClient();

                // TODO: Authentication

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientMessages));
                clientThread.Start(client);                               
            }
        }

        /// <summary>
        /// Handles messages sent by clients.
        /// </summary>
        /// <param name="client">TcpClient to handle messages for. Must be an object of TcpClient type.</param>
        private void HandleClientMessages(object client)
        {
            TcpClient tcpClient = (TcpClient) client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[MESSAGE_SIZE];
            int bytesRead;

            // TODO: Display username of connected user and datetime?
            Console.WriteLine("Client connected.");

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, MESSAGE_SIZE);
                }
                catch (Exception ex)
                {
                    // TODO: Log an exception.
                    break;
                }

                // TODO: Add a timeout. If no messages were returned for x amount of time
                // the user has disconnected. Stop the thread.

                //if(bytesRead ==  && ((lastMessageRead - currentTime) > timeout) 
                //{
                //  handle disconnect
                //}

                ASCIIEncoding encoder = new ASCIIEncoding();
                Console.WriteLine(encoder.GetString(message, 0, bytesRead));
            }

            tcpClient.Close();

            // TODO: Display username of disconnected user and datetime?
            Console.WriteLine("Client disconnected"); 
        }


        private void HandlePlayerMessage(string clientMessage, Player player, NetworkStream clientStream)
        {
            switch (clientMessage)
            {
                case "TurnLeft":
                    break;
                case "TurnRight":
                    break;
                case "Accelerate":
                    break;
                case "Decelerate":
                    break;
                case "GetPosition":
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    Byte[] message = encoder.GetBytes("Position:" + player.Position.X + "," + player.Position.Y);
                    clientStream.Write(message, 0, message.Length);
                    break;
            }
        }
    }
}
