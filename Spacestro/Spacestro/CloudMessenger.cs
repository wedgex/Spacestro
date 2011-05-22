using System;
using Lidgren.Network;
using Spacestro.Cloud.Library;

namespace Spacestro
{
    class CloudMessenger
    {
        private NetClient netClient;

        public event EventHandler<NetIncomingMessageRecievedEventArgs> MessageRecieved;

        public CloudMessenger(string configName)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(configName);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            this.netClient = new NetClient(config);           
            
            this.netClient.Start();

            //TODO need to load this from config or something.
            this.netClient.DiscoverKnownPeer("localhost", 8383);
        }

        /// <summary>
        /// Checks the net client for any recieved messages and fires the MessageRecieved Event for each.
        /// </summary>
        public void CheckForNewMessages()
        {
            NetIncomingMessage msg;
            while ((msg = this.netClient.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        // just connect to first server discovered
                        // TODO is there anything that needs to be done differently here?
                        this.netClient.Connect(msg.SenderEndpoint);
                        break;
                    case NetIncomingMessageType.Data:
                        if (this.MessageRecieved != null)
                        {
                            this.MessageRecieved(this, new NetIncomingMessageRecievedEventArgs(msg));
                        }
                        break;
                }
            }
        }

        // TODO will need overrides for the different types of messages we can send. Just doing a simple bool for now to test with.
        public void SendMessage(bool boolMsg)
        {
            NetOutgoingMessage msg = this.netClient.CreateMessage();
            msg.Write(boolMsg);
            this.netClient.SendMessage(msg, NetDeliveryMethod.Unreliable);
        }

        public void SendMessage(Cloud.Library.InputState state)
        {
            NetOutgoingMessage msg = this.netClient.CreateMessage();
            msg.Write((int)CloudMessageType.Keystate);
            msg.Write(state.Left);
            msg.Write(state.Up);
            msg.Write(state.Right);
            msg.Write(state.Down);
            this.netClient.SendMessage(msg, NetDeliveryMethod.Unreliable);
        }
    }   
}
