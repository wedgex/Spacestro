using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using Spacestro.Cloud.Library;

namespace Spacestro.Cloud
{
    class Cloud
    {
        private NetServer server;
        private double messagesPerSecond = 30.0;

        public event EventHandler<NetIncomingMessageRecievedEventArgs> MessageRecieved;

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
                                // TODO handle connection. Looks like we might can get disconnection messages here. Each connection has a Tag property as well, that we can store an object in.
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            Console.WriteLine(string.Format("Message Recivied from: {0}", msg.SenderConnection.RemoteUniqueIdentifier));
                            if (this.MessageRecieved != null)
                            {
                                this.MessageRecieved(this, new NetIncomingMessageRecievedEventArgs(msg));
                            }
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
                    nextSendUpdates += (1.0 / 30.0);
                }

                Thread.Sleep(1);
            }

            Console.WriteLine("Server stopping.");
        }            
    }
}
