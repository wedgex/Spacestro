using System;
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
        private String value = null;

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
                            //Console.WriteLine(string.Format("got msg from: " + NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier)));

                            handleMessage(msg);
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
                        // storing client id in Tag.  If it's null, ask client to send it over.
                        if (connection.Tag == null)
                        {
                            NetOutgoingMessage sendMsg = server.CreateMessage();
                            sendMsg.Write((byte)99);
                            server.SendMessage(sendMsg, connection, NetDeliveryMethod.ReliableUnordered);
                        }


                        // TODO Handle broadcasting messages to connected clients.

                    }
                    nextSendUpdates += (1.0 / messagesPerSecond);
                }

                Thread.Sleep(1);
            }

            Console.WriteLine("Server stopping.");
        }

        protected void handleMessage(NetIncomingMessage msg)
        {
            int packetId = msg.ReadByte();
            // Console.WriteLine(packetId);

            switch (packetId)
            {
                case 0: // client ID!
                    msg.SenderConnection.Tag = msg.ReadString();
                    // TODO: Read the client ID, dictionary that ho to the session id assigned by cloud.
                    Console.WriteLine(msg.SenderConnection.Tag);
                    break;
                case 1: // keyboards!
                    InputState inState = new InputState(msg.ReadByte(), msg.ReadByte(), msg.ReadByte(), msg.ReadByte());

                    break;
                default:
                    // unknown packet id
                    break;
            }
        }
    }
}
