﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using Spacestro.Cloud.Library;


// TODO   NEED TO FIGURE OUT A WAY TO ASSIGN SESSION IDS TO CLIENTS.



namespace Spacestro.Cloud
{
    class Cloud
    {
        private NetServer server;
        private double messagesPerSecond = 30.0;

        //public event EventHandler<NetIncomingMessageRecievedEventArgs> MessageRecieved;

        public Cloud(string configName, int port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(configName);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = port;

            this.server = new NetServer(config); 
        }

        public void Start()
        {
            this.server.Start();

            double nextSendUpdates = NetTime.Now;
            double now;

            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            // Respond to the discovery request.
                            server.SendDiscoveryResponse(null, msg.SenderEndpoint);
                            break;



                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            // Print the message.
                            // TODO log errors?
                            Console.WriteLine(msg.ReadString());
                            break;



                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
                                // TODO Each connection has a Tag property as well, that we can store an object in.
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                // closing client makes it timeout and after a few seconds sends this message?
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnected!");
                            }
                            else if (status == NetConnectionStatus.Disconnecting)
                            {
                                // can't get this one to trigger just yet.
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnecting!");
                            }
                            break;



                        case NetIncomingMessageType.Data:
                            Console.WriteLine(string.Format("Message Receivied from: " + NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier)));

                            handleMessage(msg.ReadByte());
                            //if (this.MessageRecieved != null)
                            //{
                            //    this.MessageRecieved(this, new NetIncomingMessageRecievedEventArgs(msg));
                            //}
                            break;
                    }
                }

                now = NetTime.Now;
                if (now > nextSendUpdates)
                {
                    foreach (NetConnection connection in server.Connections)
                    {
                        // TODO Handle broadcasting messages to connected clients.
                    }
                    nextSendUpdates += (1.0 / messagesPerSecond);
                }

                Thread.Sleep(1);
            }

            Console.WriteLine("Server stopping.");
        }
        
        protected void handleMessage(byte packetId)
        {
            switch ((int)packetId)
            {
                case 1: // keyboards!
                    
                    // TODO: Read the keyboard input, tell the game logic to update player position.

                    break;
                default:
                    // unknown packet id
                    break;
            }
        }
    }
}
