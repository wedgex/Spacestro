using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Cloud.Library;
using Spacestro.Entities;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Spacestro.Cloud
{
    public struct Collision
    {
        public Player player { get; set; }
        public Projectile projectile { get; set; }

        public Collision(Player p, Projectile proj)
            : this()
        {
            this.player = p;
            this.projectile = proj;
        }
    }

    class CloudGameController
    {
        public int worldWidth = 2000;
        public int worldHeight = 2000;

        Random random = new Random((int)NetTime.Now);

        public List<Player> playerList;
        public List<Projectile> projectiles;
        public List<Projectile> removeProjList;
        public List<Collision> collisionList;

        public List<Enemy> enemyList;
        private int maxEnemies = 10;

        private int bulletkey = 0;
        private int entitykey = 0;

        public Dictionary<long, string> pList;
        double now;
        double nextUpdate = NetTime.Now;
        private double ticksPerSecond = 20.0;

        private bool running = false;

        public CloudGameController()
        {
            playerList = new List<Player>();
            projectiles = new List<Projectile>();
            removeProjList = new List<Projectile>();
            collisionList = new List<Collision>();
            enemyList = new List<Enemy>();
            pList = new Dictionary<long, string>();
        }

        public void Stop()
        {
            this.running = false;
        }

        public void run()
        {
            this.running = true;

            while (this.running)
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
            createNewEnemy(); // hooray enemies
            moveAll();        // to be implemented for enemies
            updateFireRates();// hooray enemies
            checkCollisions();// to be implemented for enemies
            cleanLists();

        }

        private void createNewEnemy()
        {
            // check for max enemies
            if (enemyList.Count != maxEnemies)
            {
                // create new enemies
                enemyList.Add(new Enemy(new Vector2(random.Next(worldWidth), random.Next(worldHeight)), entitykey));
                entitykey++;
            }
        }

        private void checkCollisions()
        {
            foreach (Player p in playerList)
            {
                foreach (Projectile proj in projectiles)
                {
                    if (proj.Active && !p.Name.Equals(proj.Shooter))
                    {
                        if (proj.getRectangle().Intersects(p.getRectangle()))
                        {
                            collisionList.Add(new Collision(p, proj));
                            proj.Active = false;
                        }
                    }
                }
            }
        }

        public void clearCollisionsList()
        {
            collisionList = new List<Collision>();
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

            if (enemyList.Count != 0)
            {
                foreach (Enemy enemy in enemyList)
                {
                    //enemy.Move();
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
                if (player.firecounter != 0)
                    player.firecounter--;
            }

            if (enemyList.Count != 0)
            {
                foreach (Enemy enemy in enemyList)
                {
                    if (enemy.firecounter != 0)
                        enemy.firecounter--;
                }
            }
        }
    }
}
