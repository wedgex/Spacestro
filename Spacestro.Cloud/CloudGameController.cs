using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Cloud.Library;
using Spacestro.Entities;
using Lidgren.Network;

namespace Spacestro.Cloud
{
    class CloudGameController
    {
        public List<Player> playerList;
        public List<Projectile> projectiles;
        public List<Projectile> removeProjList;
        private int bulletkey = 0;
        public Dictionary<long, string> pList;
        double now;
        double nextUpdate = NetTime.Now;
        private double ticksPerSecond = 20.0;

        public CloudGameController()
        {
            playerList = new List<Player>();
            projectiles = new List<Projectile>();
            removeProjList = new List<Projectile>();
            pList = new Dictionary<long, string>();
        }

        public void run()
        {
            while (true)
            {
                now = NetTime.Now;
                if (now > nextUpdate)
                {
                    tick();
                    nextUpdate += (1.0 / ticksPerSecond);
                }
            }
        }

        // this is where all server side game logic happens every tick
        public void tick()
        {
            moveAll();
            updateFireRates();
            cleanLists();
        }

        public void addPlayer(String name, long remoteID)
        {
            Player newP = new Player();
            newP.Name = name;
            playerList.Add(newP);
            pList.Add(remoteID, name);
        }

        public void removePlayer(String name, long remoteID)
        {
            foreach (Player player in playerList)
            {
                if (player.Name.Equals(name))
                {
                    playerList.Remove(player);
                    break;
                }
            }
            pList.Remove(remoteID);
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

        public void handleInputState(InputState inState, String name)
        {
            foreach (Player player in playerList)
            {
                if (player.Name.Equals(name))
                {
                    player.handleInputState(inState);

                    if (inState.Space)
                    {
                        createBullet(player);
                    }
                    break;
                }
            }
        }

        public void move(String name)
        {
            foreach (Player player in playerList)
            {
                if (player.Name.Equals(name))
                {
                    player.Move();
                    break;
                }
            }
        }

        public void createBullet(Player player)
        {
            if (player.canShoot())
            {
                projectiles.Add(new Projectile(player.Position, player.Rotation, bulletkey, player.Name));
                player.firecounter = player.FireRate;
                bulletkey++;
            }
        }

        public void moveAll()
        {
            foreach (Player player in playerList)
            {
                player.Move();
            }

            if (projectiles.Count != 0)
            {
                foreach (Projectile proj in projectiles)
                {
                    if (proj.Active)
                    {
                        proj.Move();
                    }
                    else
                    {
                        removeProjList.Add(proj);
                    }
                }
            }
        }

        private void cleanLists()
        {
            if (removeProjList.Count != 0)
            {
                foreach (Projectile proj in removeProjList)
                {
                    projectiles.Remove(proj);
                }
            }
        }

        private void updateFireRates()
        {
            foreach (Player player in playerList)
            {
                if (player.firecounter == 0)
                    return;
                else
                    player.firecounter--;
            }
        }
    }
}
