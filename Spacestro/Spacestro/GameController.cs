using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Entities;
using Microsoft.Xna.Framework;

namespace Spacestro
{
    public class GameController
    {
        public List<Player> playerList;
        public List<Projectile> projectiles;
        public List<Enemy> enemies;

        public GameController()
        {
            playerList = new List<Player>();
            projectiles = new List<Projectile>();
            enemies = new List<Enemy>();
        }

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

        public void createBullet(Projectile proj)
        {
            lock (this.projectiles)
            {
                projectiles.Add(proj);
            }
        }

        public List<Enemy> getEnemyListCopy()
        {
            List<Enemy> copy;
            lock (this.enemies)
            {
                copy = this.enemies.ToList();
            }
            return copy;
        }

        public bool inPlayerList(String cid)
        {
            lock (this.playerList)
            {
                foreach (Player p in playerList)
                {
                    if (p.Name.Equals(cid))
                        return true;
                }
            }
            return false;
        }

        public bool inProjectileList(int id)
        {
            foreach (Projectile p in projectiles)
            {
                if (p.ID == id)
                    return true;
            }
            return false;
        }

        public bool inEnemyList(int id)
        {
            foreach (Enemy e in enemies)
            {
                if (e.ID == id)
                    return true;
            }
            return false;
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

        public void removePlayer(String name)
        {
            lock (this.playerList)
            {
                foreach (Player player in playerList)
                {
                    if (player.Name.Equals(name))
                    {
                        playerList.Remove(player);
                        return;
                    }
                }
            }
        }

        public void updatePlayerServerPosition(string id, float x, float y, float rot)
        {
            lock (this.playerList)
            {
                foreach (Player player in this.playerList)
                {
                    if (player.Name.Equals(id))
                    {
                        player.setSvrPosRot(x, y, rot);
                        return;
                    }
                }
            }
        }

        public void updateOrCreateProjectile(int key, float x, float y, float rot, string shooter)
        {
            lock (this.projectiles)
            {
                foreach (Projectile proj in this.projectiles)
                {
                    if (proj.ID == key)
                    {
                        proj.setSvrPosRot(x, y, rot);
                        return;
                    }
                }
                projectiles.Add(new Projectile(new Vector2(x, y), rot, key, shooter));
            }
        }

        public void updateOrCreateEnemy(int key, float x, float y, float rot)
        {
            lock (this.enemies)
            {
                foreach (Enemy e in this.enemies)
                {
                    if (e.ID == key)
                    {
                        e.setSvrPosRot(x, y, rot);
                        return;
                    }
                }
                enemies.Add(new Enemy(new Vector2(x, y), key));
            }
        }

        public void hitPlayer(string id)
        {
            lock (this.playerList)
            {
                foreach (Player player in this.playerList)
                {
                    if (player.Name.Equals(id))
                    {
                        player.getHit();
                        return;
                    }
                }
            }
        }

        public void hitEnemy(int id)
        {
            foreach (Enemy e in enemies)
            {
                if (e.ID == id)
                    e.getHit();
            }
        }

        public void destroyBullet(int key)
        {
            lock (this.projectiles)
            {
                Projectile tempP = null;
                foreach (Projectile proj in this.projectiles)
                {
                    if (proj.ID == key)
                    {
                        tempP = proj;
                        break;
                    }
                }
                if (tempP != null)
                    projectiles.Remove(tempP);
            }
        }

        public void addPlayer(string id)
        {
            lock (this.playerList)
            {
                Player newP = new Player();
                newP.Name = id;
                playerList.Add(newP);
            }
        }

        public Projectile getProjectile(int id)
        {
            foreach (Projectile p in projectiles)
            {
                if (p.ID == id)
                    return p;
            }
            return null;
        }

        public Enemy getEnemy(int id)
        {
            foreach (Enemy e in enemies)
            {
                if (e.ID == id)
                    return e;
            }
            return null;
        }
    }
}
