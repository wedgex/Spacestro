using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Spacestro.Cloud.Library;
using Spacestro.Entities;

namespace Spacestro.Cloud
{
    class Cloud
    {
        private NetServer server;
        private double messagesPerSecond = 10.0;
        private Player p1;
        private InputState inState;
        private CloudGameController cloudGC;

        public Cloud(string configName, int port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(configName);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = port;

            this.server = new NetServer(config);

            cloudGC = new CloudGameController();

            Thread thread = new Thread(new ThreadStart(cloudGC.run));
            thread.Start();
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

                        case NetIncomingMessageType.Data:  // handle message from client

                            //Console.WriteLine(string.Format("got msg from: " + NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier)));
                            handleMessage(msg);
                            break;
                    }
                }

                now = NetTime.Now;
                if (now > nextSendUpdates)
                {
                    foreach (NetConnection connection in server.Connections)
                    {
                        // storing client id in Tag.  If it's null, ask client to send it over.
                        // Also send client it's session id.
                        if (connection.Tag == null)
                        {
                            NetOutgoingMessage sendMsg = server.CreateMessage();
                            sendMsg.Write((byte)99);
                            server.SendMessage(sendMsg, connection, NetDeliveryMethod.ReliableUnordered);
                        }
                        else
                        {
                            // tell player of everyone's position including itself.
                            foreach (NetConnection player in server.Connections)
                            {
                                if (player.Tag != null)
                                {
                                    NetOutgoingMessage sendMsg = server.CreateMessage();
                                    sendMsg.Write((byte)5); // packet id
                                    sendMsg.Write(player.Tag.ToString()); // client id
                                    p1 = cloudGC.getPlayer(player.Tag.ToString());
                                    sendMsg.Write(p1.Position.X);
                                    sendMsg.Write(p1.Position.Y);
                                    sendMsg.Write(p1.Rotation);
                                    server.SendMessage(sendMsg, connection, NetDeliveryMethod.Unreliable);
                                }
                            }
                        }
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
                    if (msg.SenderConnection.Tag == null)
                    {
                        msg.SenderConnection.Tag = msg.ReadString();
                        Console.WriteLine(msg.SenderConnection.Tag.ToString());
                        cloudGC.addPlayer(msg.SenderConnection.Tag.ToString());
                    }
                    break;
                case 1: // keyboards!
                    inState.resetStates();
                    inState.setStates(msg.ReadByte(), msg.ReadByte(), msg.ReadByte(), msg.ReadByte());
                    cloudGC.handleInputState(inState, msg.SenderConnection.Tag.ToString());
                    // tell player new position
                    break;
                default:
                    // unknown packet id
                    break;
            }
        }
    }
}
