using System;
using System.IO;
using Lidgren.Network;
using Spacestro.Cloud.Library;
using Microsoft.Xna.Framework;
using Spacestro.Entities;
using System.Collections.Generic;

namespace Spacestro
{
    class CloudMessenger
    {
        private NetClient netClient;
        private String svrID = "";

        //private String client_id = "dereksucks420";
        public String client_id = Path.GetRandomFileName().Replace(".", "");  // creates random string; also is awesome
        public List<Player> playerList;

        public CloudMessenger(string configName)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(configName);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            this.netClient = new NetClient(config);
            this.netClient.Start();

            //TODO need to load this from config or something.
            this.netClient.DiscoverKnownPeer("localhost", 8383);

            this.playerList = new List<Player>();
        }

        /// <summary>
        /// Checks the net client for any recieved messages
        /// </summary>
        public void CheckForNewMessages()
        {
            NetIncomingMessage msg;
            while ((msg = this.netClient.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        this.netClient.Connect(msg.SenderEndpoint);
                        break;
                    case NetIncomingMessageType.Data:
                        handleMessage(msg);
                        break;
                }
            }
        }

        protected void handleMessage(NetIncomingMessage msg)
        {
            int packetId = msg.ReadByte();

            switch (packetId)
            {
                case 99: // server wants client ID!
                    SendMessage(client_id);
                    break;
                    
                case 1: // player disconnected
                    playerList.Remove(getPlayer(msg.ReadString()));
                    break;

                case 5: // position and rotation of some player (maybe us)
                    svrID = msg.ReadString();

                    // not in list yet so we need to create a new player entity
                    if (!inPlayerList(svrID))
                    {
                        Player newP = new Player();
                        newP.Name = svrID;
                        playerList.Add(newP);
                    }
                    // in list already so just update
                    else if (inPlayerList(svrID))
                    {
                        getPlayer(svrID).Position = new Vector2(msg.ReadFloat(), msg.ReadFloat());
                        getPlayer(svrID).Rotation = msg.ReadFloat();
                    }
                    break;

                default:
                    // unknown packet id
                    break;
            }
        }

        // PacketID 0
        // sends client ID to the server
        public void SendMessage(String id)
        {
            NetOutgoingMessage msg = this.netClient.CreateMessage();
            msg.Write((byte)0);
            msg.Write(id);
            this.netClient.SendMessage(msg, NetDeliveryMethod.Unreliable);
        }

        // PacketID 1
        // sends keyboard message 
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

        private bool inPlayerList(String cid)
        {
            foreach (Player p in playerList)
            {
                if (p.Name.Equals(cid))
                    return true;
            }
            return false;
        }

        public Player getPlayer(String name)
        {
            foreach (Player player in playerList)
            {
                if (player.Name.Equals(name))
                {
                    return player;
                }
            }

            return null;
        }
    }
}
