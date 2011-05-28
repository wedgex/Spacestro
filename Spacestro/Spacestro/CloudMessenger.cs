using System;
using Lidgren.Network;
using Spacestro.Cloud.Library;




// TODO   CREATE A UNIQUE ID FOR THIS CLIENT SO SERVER KNOWS WHO THE HELL THIS IS.



namespace Spacestro
{
    class CloudMessenger
    {
        private NetClient netClient;

        //public event EventHandler<NetIncomingMessageRecievedEventArgs> MessageRecieved;

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

                        // read message for new position

                        //if (this.MessageRecieved != null)
                        //{
                        //    this.MessageRecieved(this, new NetIncomingMessageRecievedEventArgs(msg));
                        //}
                        break;
                }
            }
        }

        // TODO will need overrides for the different types of messages we can send. Just doing a simple bool for now to test with.
        //public void SendMessage(bool boolMsg)
        //{
        //    NetOutgoingMessage msg = this.netClient.CreateMessage();
        //    msg.Write(boolMsg);
        //    this.netClient.SendMessage(msg, NetDeliveryMethod.Unreliable);
        //}

        // sends keyboard message PacketID 1
        public void SendMessage(Cloud.Library.InputState state)
        {
            NetOutgoingMessage msg = this.netClient.CreateMessage();
            msg.Write((byte)1);
            foreach (bool key in state.getStateList())
            {
                if (key)
                {
                    msg.Write((byte)1);
                }
                else
                {
                    msg.Write((byte)0);
                }
            }
            this.netClient.SendMessage(msg, NetDeliveryMethod.Unreliable);
        }
    }
}
