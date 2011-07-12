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
        private string svrID = "";
        private NetClient netClient;
        
        public GameController GameController { get; private set; }
        public string ClientID { get; private set; }
        
        public bool Connected 
        {
            get
            {
                NetIncomingMessage msg;
                while ((msg = this.netClient.ReadMessage()) != null)
                {
                    if (msg.MessageType == NetIncomingMessageType.DiscoveryResponse)
                    {
                        this.netClient.Connect(msg.SenderEndpoint);
                        return true;
                    }
                }
                return false;
            }
        }

        public void Stop()
        {
            this.netClient.Shutdown("Messenger Shutting down");
        }

        public CloudMessenger(string configName, string ipAddress)
        {
            this.ClientID = Path.GetRandomFileName().Replace(".", "");  // creates random string; also is awesome

            NetPeerConfiguration config = new NetPeerConfiguration(configName);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.NetworkThreadName = "Cloud Messenger thread";

            this.netClient = new NetClient(config);
            this.netClient.Start();

            this.netClient.DiscoverKnownPeer(ipAddress, 8383);

            this.GameController = new GameController();
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
                        HandleMessage(msg);
                        break;
                }
            }
        }

        protected void HandleMessage(NetIncomingMessage msg)
        {
            int packetId = msg.ReadByte();

            switch (packetId)
            {
                case 99: // server wants client ID!
                    SendMessage(this.ClientID);
                    break;

                case 1: // player disconnected
                    this.GameController.removePlayer(msg.ReadString());
                    break;

                case 5: // position and rotation of some player (maybe us)
                    svrID = msg.ReadString();

                    // not in list yet so we need to create a new player entity
                    if (!this.GameController.inPlayerList(svrID))
                    {
                        Player newP = new Player();
                        newP.Name = svrID;
                        this.GameController.addPlayer(newP);
                    }
                    // in list already so just update
                    else if (this.GameController.inPlayerList(svrID))
                    {
                        if (this.ClientID == svrID) // this is us!
                        {
                            // cool story bro!
                        }

                        this.GameController.getPlayer(svrID).setSvrPosRot(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    }
                    break;

                case 10: // projectile
                    int key = msg.ReadByte();
                    if (this.GameController.inProjectileList(key))
                    {
                        this.GameController.getProjectile(key).setSvrPosRot(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    }
                    else
                    {
                        float f1, f2, f3;
                        f1 = msg.ReadFloat();
                        f2 = msg.ReadFloat();
                        f3 = msg.ReadFloat();
                        this.GameController.projectiles.Add(new Projectile(new Vector2(f1, f2), f3, key, msg.ReadString()));
                    }
                    break;

                case 11: // enemy
                    int ekey = msg.ReadByte();
                    if (this.GameController.inEnemyList(ekey))
                    {
                        this.GameController.getEnemy(ekey).setSvrPosRot(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    }
                    else
                    {
                        float f1, f2, f3;
                        f1 = msg.ReadFloat();
                        f2 = msg.ReadFloat();
                        f3 = msg.ReadFloat();
                        this.GameController.enemies.Add(new Enemy(new Vector2(f1, f2), ekey));
                    }
                    break;

                case 15: // collision
                    int tempID = msg.ReadByte();
                    switch (tempID)
                    {
                        case 1: // player on player
                            // we don't mess with velocity here since we get that as seperate packet.
                            // we'll probably use this spot for animation or whatever.
                            break;
                        case 2: // player on bullet
                            this.GameController.getPlayer(msg.ReadString()).getHit();
                            int projID = msg.ReadByte();
                            if (this.GameController.inProjectileList(projID))
                            {
                                this.GameController.getProjectile(projID).Active = false;
                            }
                            break;
                        case 3: // player on enemy
                            // same as case 1
                            break;
                        case 4: // enemy on bullet
                            this.GameController.getEnemy(msg.ReadByte()).getHit();
                            int pid = msg.ReadByte();
                            if (this.GameController.inProjectileList(pid) && pid != 0)
                            {
                                this.GameController.getProjectile(pid).Active = false;
                            }
                            break;
                        default:
                            break;
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
    }
}
