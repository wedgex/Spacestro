using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Cloud.Library;
using Spacestro.Entities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Spacestro.Cloud.AI;

namespace Spacestro.Cloud
{
    public struct Collision
    {
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public Projectile projectile { get; set; }
        public Enemy enemy { get; set; }
        public int CID { get; set; }

        public Collision(Object obj1, Object obj2)
            : this()
        {
            if (obj1 is Player)
            {
                if (obj2 is Player)
                {
                    this.CID = 2;
                    this.player1 = (Player)obj1;
                    this.player2 = (Player)obj2;
                }
                else if (obj2 is Projectile)
                {
                    this.CID = 2;
                    this.player1 = (Player)obj1;
                    this.projectile = (Projectile)obj2;
                }
                else if (obj2 is Enemy)
                {
                    this.CID = 3;
                    this.player1 = (Player)obj1;
                    this.enemy = (Enemy)obj2;
                }
            }
            else if (obj1 is Enemy)
            {
                if (obj2 is Projectile)
                {
                    this.CID = 4;
                    this.enemy = (Enemy)obj1;
                    this.projectile = (Projectile)obj2;
                }
            }
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
        private int aggroRange = 500;

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
            updateFireRates();
            createNewEnemy();
            checkAggroRange();
            moveAll();
            enemiesShoot();
            checkCollisions();
            handleCollisions();
            cleanLists();

        }

        private void enemiesShoot()
        {
            foreach (Enemy e in enemyList)
            {
                if (e.canShoot() && !e.TargetPlayer.Equals(""))
                {
                    float angletofire = AIUtil.getAnglePredictPlayerPos(e, getPlayer(e.TargetPlayer));

                    if (angletofire == -1) // can't hit player so skip
                        continue;

                    // we can hit player to fire away!
                    projectiles.Add(new Projectile(e.Position, angletofire, bulletkey, e.ID.ToString()));
                    e.firecounter = e.FireRate;
                    bulletkey++;

                }
            }
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

        private void checkAggroRange()
        {
            foreach (Enemy e in enemyList)
            {
                if (e.TargetPlayer.Equals(""))
                {
                    foreach (Player p in playerList)
                    {
                        if (Vector2.Distance(p.Position, e.Position) < aggroRange)
                        {
                            e.TargetPlayer = p.Name;
                            break;
                        }
                    }
                }
            }
        }

        /*
         * collisions checked in this order:
         * 
         * for every player ->
         *   check against other players
         *   check against bullets
         *   check against enemies
         *   
         * for every enemy ->
         *   check against bullets
         */
        private void checkCollisions()
        {
            // for every player
            foreach (Player p in playerList)
            {
                // check against players
                foreach (Player pcheck in playerList)
                {
                    if (p.Name != pcheck.Name && p.getRectangle().Intersects(pcheck.getRectangle()))
                    {
                        collisionList.Add(new Collision(p, pcheck));
                    }
                }

                // check against bullets
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

                // check against enemies
                foreach (Enemy e in enemyList)
                {
                    if (e.getRectangle().Intersects(p.getRectangle()))
                    {
                        collisionList.Add(new Collision(p, e));
                    }
                }
            }

            // for every enemy
            foreach (Enemy e in enemyList)
            {
                // check against bullets
                foreach (Projectile proj in projectiles)
                {
                    if (e.getRectangle().Intersects(proj.getRectangle()))
                    {
                        collisionList.Add(new Collision(e, proj));
                        proj.Active = false;
                    }
                }
            }
        }

        public void handleCollisions()
        {
            // we have a collision list full of this ticks collisions.  do shit with it.
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
                    if (!enemy.TargetPlayer.Equals(""))
                    {
                        enemy.Rotation = AIUtil.getAnglePointingAt(enemy.Position, getPlayer(enemy.TargetPlayer).Position);
                        enemy.Accelerate();
                        enemy.Move();
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
