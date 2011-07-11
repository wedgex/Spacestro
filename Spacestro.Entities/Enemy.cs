using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro.Entities
{
    public class Enemy : Entity
    {
        public int ID { get; set; }
        public string TargetPlayer { get; set; }
        public bool Active;

        public Enemy()
        {
            this.Acceleration = 0.2f;
            this.Velocity = Vector2.Zero;
            this.TurnSpeed = 0.1f;
            this.MaxSpeed = 5.0f;
            this.Rotation = 0.0f;
            this.FireRate = 15;
            this.TargetPlayer = "";
            this.Active = true;
        }

        public Enemy(Vector2 position, int id)
            : this()
        {
            this.Position = position;
            this.ID = id;
        }

        public void getHit()
        {
            this.Active = false;
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)(this.Position.X - (35 / 2)), (int)(this.Position.Y - (35 / 2)), 35, 35);
        }
    }
}
