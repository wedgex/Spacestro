using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Entities;

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

        public bool inPlayerList(String cid)
        {
            foreach (Player p in playerList)
            {
                if (p.Name.Equals(cid))
                    return true;
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
            foreach (Player player in playerList)
            {
                if (player.Name.Equals(name))
                {
                    return player;
                }
            }
            return null;
        }

        public void removePlayer(String name)
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

        public void addPlayer(Player p)
        {
            playerList.Add(p);
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
