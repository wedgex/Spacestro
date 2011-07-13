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
                    this.CID = 1;
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
        public List<Enemy> enemyList;
        public List<Collision> collisionList;

        private List<Projectile> removeProjList;
        private List<Enemy> removeEnemyList;

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
            removeEnemyList = new List<Enemy>();
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
            updatePlayerCollisionTickers();
            createNewEnemy();
            checkAggroRange();
            moveAll();
            enemiesShoot();
            checkCollisions();
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
                else
                {
                    if (getPlayer(e.TargetPlayer) == null)
                    {
                        e.TargetPlayer = "";
                    }
                }
            }
        }

        /*
         * collisions checked in this order:
         * 
         * for every player ->
         *   check against other players (CID 1)
         *   check against bullets (CID 2)
         *   check against enemies (CID 3)
         *   
         * for every enemy ->
         *   check against bullets (CID 4)
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
                        // make sure we haven't collided recently
                        if (!p.collidedWith.ContainsKey(pcheck.Name))
                        {
                            p.collidedWith.Add(pcheck.Name, 10);
                            pcheck.collidedWith.Add(p.Name, 10);
                            collisionList.Add(new Collision(p, pcheck));
                            p.Velocity = p.Velocity * -0.5f;
                            pcheck.Velocity = pcheck.Velocity * -0.5f;
                        }
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
                        // make sure we haven't collided recently
                        if (!p.collidedWith.ContainsKey(e.ID.ToString()))
                        {
                            p.collidedWith.Add(e.ID.ToString(), 10);
                            collisionList.Add(new Collision(p, e));
                            p.Velocity = p.Velocity * -0.5f;
                            e.Velocity = e.Velocity * -0.5f;
                        }
                    }
                }
            }

            // for every enemy
            foreach (Enemy e in enemyList)
            {
                // check against bullets
                foreach (Projectile proj in projectiles)
                {
                    if (proj.Active && !proj.Shooter.Equals("enemy"))
                    {
                        if (e.getRectangle().Intersects(proj.getRectangle()))
                        {
                            collisionList.Add(new Collision(e, proj));
                            proj.Active = false;
                            e.getHit();
                        }
                    }
                }
            }
        }

        public void addPlayer(String name, long remoteID)
        {
            lock (this.playerList)
            {
                Player newP = new Player();
                newP.Name = name;
                playerList.Add(newP);
                pList.Add(remoteID, name);
            }
        }

        public void removePlayer(String name, long remoteID)
        {
            Player tempPlayer = getPlayer(name);
            lock (this.playerList)
            {
                this.playerList.Remove(tempPlayer);
                pList.Remove(remoteID);
            }
        }

        public Player getPlayer(String name)
        {
            lock (this.playerList)
            {
                foreach (Player player in this.playerList)
                {
                    if (player.Name.Equals(name))
                    {
                        return player;
                    }
                }
            }
            return null;
        }

        public void handleInputState(InputState inState, String name)
        {
            getPlayer(name).handleInputState(inState);

            if (inState.Space)
            {
                this.createBullet(getPlayer(name));
            }
        }

        public void move(String name)
        {
            getPlayer(name).Move();
        }

        public void createBullet(Player player)
        {
            if (player.canShoot())
            {
                lock (this.projectiles)
                {
                    projectiles.Add(new Projectile(player.Position, player.Rotation, this.bulletkey, player.Name));
                }
                player.firecounter = player.FireRate;
                this.bulletkey++;
            }
        }

        private void enemiesShoot()
        {
            foreach (Enemy e in enemyList)
            {
                if (e.canShoot() && !e.TargetPlayer.Equals(""))
                {
                    float angletofire = AIUtil.getAnglePredictPlayerPos(e, getPlayer(e.TargetPlayer));

                    if (angletofire == -1) // can't hit player mathematically so skip
                        continue;

                    // we can hit player to fire away!
                    projectiles.Add(new Projectile(e.Position, angletofire, this.bulletkey, "enemy"));
                    e.firecounter = e.FireRate;
                    this.bulletkey++;
                }
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
                    if (enemy.Active)
                    {
                        if (!enemy.TargetPlayer.Equals(""))
                        {
                            enemy.Rotation = AIUtil.getAnglePointingAt(enemy.Position, getPlayer(enemy.TargetPlayer).Position);
                            enemy.Accelerate();
                            enemy.Move();
                        }
                    }
                    else { removeEnemyList.Add(enemy); }
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

            if (removeEnemyList.Count != 0)
            {
                foreach (Enemy e in removeEnemyList)
                {
                    enemyList.Remove(e);
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

        private void updatePlayerCollisionTickers()
        {
            foreach (Player player in playerList)
            {
                player.tickDownCollidedWithList();
            }
        }

        // THREAD SAFETY ALL UP IN THIS BITCH
        //public List<Player> playerList;
        //public List<Projectile> projectiles;
        //public List<Enemy> enemyList;
        //public List<Collision> collisionList;

        public List<Player> getPlayerListCopy()
        {
            List<Player> copy;
            lock (this.playerList)
            {
                copy = this.playerList.ToList();
            }
            return copy;
        }

        public List<Projectile> getProjectileListCopy()
        {
            List<Projectile> copy;
            lock (this.playerList)
            {
                copy = this.projectiles.ToList();
            }
            return copy;
        }

        public List<Enemy> getEnemyListCopy()
        {
            List<Enemy> copy;
            lock (this.enemyList)
            {
                copy = this.enemyList.ToList();
            }
            return copy;
        }

        public List<Collision> getCollisionListCopy()
        {
            List<Collision> copy;
            lock (this.collisionList)
            {
                copy = this.collisionList.ToList();
                this.collisionList.Clear();
            }
            return copy;
        }
    }
}
