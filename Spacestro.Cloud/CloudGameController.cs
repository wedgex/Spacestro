using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Cloud.Library;
using Spacestro.Entities;

namespace Spacestro.Cloud
{
    class CloudGameController
    {
        List<Player> playerList;

        public CloudGameController()
        {
            playerList = new List<Player>();
        }

        public void tick()
        {
            // handle one tick of server
            // probably for our AI entities: update position/do action

            moveAll();
        }

        public void addPlayer(String name)
        {
            Player newP = new Player();
            newP.Name = name;
            playerList.Add(newP);
        }

        public void removePlayer(String name)
        {
            foreach (Player player in playerList)
            {
                if (player.Name.Equals(name))
                {
                    playerList.Remove(player);
                    break;
                }
            }
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

        public void moveAll()
        {
            foreach (Player player in playerList)
            {
                player.Move();
            }
        }
    }
}
