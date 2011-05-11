using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spacestro.game_obj;

namespace Spacestro
{
    class GameMath
    {
        internal static void increasePlayerSpeed(Player player)
        {  
            Vector2 tempV = new Vector2(player.speed * (float)Math.Cos(player.rotation), player.speed * (float)Math.Sin(player.rotation));
            tempV += player.velocity;
            if (tempV.Length() >= player.maxspeed)
            {
                tempV.Normalize();
                tempV = new Vector2(tempV.X * player.maxspeed, tempV.Y * player.maxspeed);
            }
            player.velocity = tempV;
        }

        internal static void decreasePlayerSpeed(Player player)
        {
            if (player.velocity.Length() <= 0.1f)
            {
                player.velocity = Vector2.Zero;
                return;
            }
            Vector2 newV = player.velocity;
            newV.Normalize();
            player.velocity += new Vector2(-player.speed * newV.X, -player.speed * newV.Y);
        }

        internal static void turnLeft(Player player)
        {
            player.rotation -= player.turnspeed;
        }

        internal static void turnRight(Player player)
        {
            player.rotation += player.turnspeed;
        }
    }
}